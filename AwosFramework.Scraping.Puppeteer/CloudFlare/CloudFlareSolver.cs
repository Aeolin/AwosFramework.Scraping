using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public class CloudFlareSolver : ICloudFlareSolver
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
			_browser = await extra.LaunchAsync(new LaunchOptions
			{
				Headless = true
			});
		}

		public async Task<bool> SolveAsync(ICloudFlareChallenge challenge, CloudFlareDataStore data)
		{
			if (challenge is JavaScriptChallenge jsChallenge)
				return await SolveJavaScriptAsync(jsChallenge, data);
		}

		private async Task<bool> SolveJavaScriptAsync(JavaScriptChallenge jsChallenge, CloudFlareDataStore data)
		{
			await _initTask;
			await _browserSemaphore.WaitAsync();
			try
			{
				var page = await _browser.NewPageAsync();
				await page.SetContentAsync(jsChallenge.Challenge);
				var cookies = await page.GetCookiesAsync();
				var ua = await _browser.GetUserAgentAsync();
				string ray = null;
				page.RequestFinished += (s, req) =>
				{
					if (req.Request.Headers.ContainsKey("Cf-Ray"))
						ray = req.Request.Headers["Cf-Ray"];
				};

				await page.WaitForNavigationAsync();
				var clearance = cookies.FirstOrDefault(x => x.Name == "cf_clearance")?.Value;
				if (clearance == null)
					return false;

				data.SetCloudFlareData(jsChallenge.Domain, ray, clearance, ua);
				return true;
			}
			finally
			{
				_browserSemaphore.Release();
			}
		}

	}
}
