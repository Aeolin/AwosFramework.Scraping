using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public class CloudFlareSolver : ICloudFlareSolver, IDisposable
	{
		private IBrowser _browser;
		private readonly SemaphoreSlim _browserSemaphore = new SemaphoreSlim(1);
		private readonly Task _initTask;

		public CloudFlareSolver()
		{
			_initTask = InitializeAsync();
		}

		private async Task InitializeAsync()
		{
			var extra = new PuppeteerExtra();
			extra.Use(new StealthPlugin());
			var fetcher = new BrowserFetcher(SupportedBrowser.Chromium);
			var browser = await fetcher.DownloadAsync(BrowserTag.Latest);
			_browser = await extra.LaunchAsync(new LaunchOptions
			{
				Browser = SupportedBrowser.Chromium,
				ExecutablePath = browser.GetExecutablePath(),
				Headless = false
			});
		}

		public async Task<CloudFlareData> SolveAsync(ICloudFlareChallenge challenge)
		{
			if (challenge is JavaScriptChallenge jsChallenge)
				return await SolveJavaScriptAsync(jsChallenge);

			return null;
		}

		private async Task<CloudFlareData> SolveJavaScriptAsync(JavaScriptChallenge jsChallenge)
		{
			await _initTask;
			await _browserSemaphore.WaitAsync();
			try
			{
				var page = await _browser.NewPageAsync();
				string ray = null;
				page.Response += (s, response) =>
				{
					response.Response.Headers.TryGetValue("Cf-Ray", out ray);
				};

				await page.GoToAsync(jsChallenge.Domain.ToString(), WaitUntilNavigation.Networkidle0);

				var iframe = await page.WaitForSelectorAsync("iframe");
				if (iframe != null)
				{
					var contentFrame = await iframe.ContentFrameAsync();
					var checkbox = await contentFrame.WaitForSelectorAsync("input[type='checkbox']");
					if (checkbox != null)
					{
						await Task.Delay(Random.Shared.Next(50, 250));
						await checkbox.ClickAsync();
						await page.WaitForNetworkIdleAsync();
					}
				}

				var cookies = await page.GetCookiesAsync();
				var userAgent = await _browser.GetUserAgentAsync();

				var cookie = string.Join("; ", cookies.Select(x => $"{x.Name}={x.Value}"));
				await page.CloseAsync();
				if (cookies.Any(x => x.Name == "cf_clearance") == false)
					return null;

				return new CloudFlareData(jsChallenge.Domain, ray, cookie, userAgent);
			}
			catch (Exception ex)
			{
				return null;
			}
			finally
			{
				_browserSemaphore.Release();
			}
		}

		public void Dispose()
		{
			_browser.Dispose();
		}
	}
}
