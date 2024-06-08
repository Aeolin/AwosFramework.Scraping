using AwosFramework.Scraping.Middleware;
using HtmlAgilityPack;
using PuppeteerSharp;

namespace AwosFramework.Scraping.PuppeteerRequestor
{
	public class PuppeteerRequestMiddlware : IMiddleware
	{
		private readonly IBrowserPool _browserPool;

		public PuppeteerRequestMiddlware(IBrowserPool browserPool)
		{
			_browserPool=browserPool;
		}

		public async Task<bool> ExecuteAsync(MiddlewareContext context)
		{
			var browser = await _browserPool.GetBrowserAsync();
			
			try
			{
				var page = await browser.NewPageAsync();
				await page.GoToAsync(context.ScrapeJob.Uri.ToString(), WaitUntilNavigation.Networkidle2);
				var dom = await page.GetContentAsync();
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(dom);
				context.AddComponent(htmlDoc);
				return true;
			}
			finally
			{
				await _browserPool.ReturnBrowserAsync(browser);
			}
		}
	}
}
