using AwosFramework.Scraping.PuppeteerRequestor.CloudFlare.Abstraction;
using AwosFramework.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public class CloudFlareSolver : ICloudFlareSolver, IDisposable
	{
		private IBrowser _browser;
		private readonly SemaphoreSlim _browserSemaphore = new SemaphoreSlim(1);
		private readonly Task _initTask;
		private readonly LaunchOptions _launchOptions;
		private readonly ILogger _logger;

		public CloudFlareSolver(LaunchOptions launcOptions = null, ILoggerFactory factory = null)
		{
			_logger = factory?.CreateLogger<CloudFlareSolver>();
			_launchOptions = launcOptions ?? new LaunchOptions
			{
				Browser = SupportedBrowser.Chromium,
				Headless = false
			};

			_initTask = InitializeAsync(factory);
		}

		private async Task InitializeAsync(ILoggerFactory factory)
		{
			var extra = new PuppeteerExtra();
			extra.Use(new StealthPlugin());
			if (_launchOptions.ExecutablePath == null)
			{
				_logger?.LogInformation("Donwloading browser...");
				var fetcher = new BrowserFetcher(new BrowserFetcherOptions
				{
					Browser = _launchOptions.Browser,
					Path = "./cf-solver-cache"
				});

				var browser = await fetcher.DownloadAsync(BrowserTag.Latest);
				_launchOptions.ExecutablePath = browser.GetExecutablePath();
				_logger?.LogInformation("Downloaded to {path}", _launchOptions.ExecutablePath);
			}
			_browser = await extra.LaunchAsync(_launchOptions, factory);
		}

		public async Task<CloudFlareClearance> SolveAsync(ICloudFlareChallenge challenge)
		{

			switch (challenge.Type)
			{
				case ChallengeType.None:
					return null;

				case ChallengeType.JavaScript:
					return await SolveJavaScriptAsync(challenge);

				default:
					return null;
			}
		}

		private async Task<CloudFlareClearance> SolveJavaScriptAsync(ICloudFlareChallenge challenge)
		{
			await _initTask;
			await _browserSemaphore.WaitAsync();
			var loggerScope = _logger?.BeginScope("CloudFlareSolver[{domain}]", challenge.Url.Host);
			_logger?.LogInformation("Begin solving JavaScript challenge");
			try
			{
				string ray = null;
				var cookieSource = new TaskCompletionSource<CookieContainer>();
				var page = (await _browser.PagesAsync()).First();

				page.Response += (s, response) =>
				{
					_logger?.LogDebug("Got response: {response}, status: {status}", response.Response.Url, response.Response.Status);
					if (ray == null && response.Response.Headers.TryGetValue("Cf-Ray", out ray))
						_logger?.LogInformation("Fetched Cf-Ray: {ray}", ray);

					if (response.Response.Headers.TryGetValue("Set-Cookie", out var cookieHeader))
					{
						var cookies = cookieHeader.ReplaceLineEndings("\n").Split("\n");

						var container = new CookieContainer();
						foreach (var cookie in cookies)
							container.SetCookies(challenge.Url, cookie);

						var collection = container.GetCookies(challenge.Url);
						if (collection.Any(x => x.Name == "cf_clearance"))
						{
							_logger?.LogInformation("Fetched cf_clearance: {clearance}", cookieHeader);
							cookieSource.TrySetResult(container);
						}
					}
				};

				await page.GoToAsync(challenge.Url.ToString(), WaitUntilNavigation.Networkidle0);
				try
				{
					var iframe = await page.WaitForSelectorAsync("iframe");
					if (iframe != null)
					{
						var contentFrame = await iframe.ContentFrameAsync();
						_logger?.LogInformation("Found challenge IFrame {source}", contentFrame.Page.Url);
						var checkbox = await contentFrame.WaitForSelectorAsync("input[type='checkbox']");
						if (checkbox != null)
						{
							_logger?.LogInformation("Found checkbox {element}", checkbox);
							await checkbox.ClickAsync();
							_logger?.LogInformation("Clicked checkbox");
						}
					}
				}
				catch (PuppeteerException ex)
				{
					_logger?.LogError(ex, $"Error while clicking checkbox occured");
					await Task.Delay(3000);
				}

				var userAgent = await _browser.GetUserAgentAsync();
				var cookieTask = await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(30)), cookieSource.Task);
				//await page.CloseAsync();
				await page.GoToAsync("about:blank");
				if (cookieTask is not Task<CookieContainer> cookies)
				{
					_logger?.LogWarning("Failed to solve challenge for due to timeout");
					return null;
				}

				var cookieContainer = cookies.Result;
				var collection = cookieContainer.GetCookies(challenge.Url);
				var cookieHeader = cookieContainer.GetCookieHeader(challenge.Url);
				var clearanceCookie = collection.First(x => x.Name == "cf_clearance");
				_logger?.LogInformation("Solved challenge, clearance: {clearance}, expiry: {expiry}", clearanceCookie.Value, clearanceCookie.Expires);
				return new CloudFlareClearance(challenge.Url, ray, cookieHeader, userAgent, clearanceCookie.Expires);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, "Failed to solve challenge for {domain}", challenge.Url);
				return null;
			}
			finally
			{
				_browserSemaphore.Release();
				loggerScope?.Dispose();
			}
		}

		public void Dispose()
		{
			_browser?.Dispose();
		}
	}
}
