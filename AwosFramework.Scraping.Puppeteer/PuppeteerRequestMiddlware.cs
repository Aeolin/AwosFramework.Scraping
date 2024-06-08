using AwosFramework.Scraping.Middleware;
using AwosFramework.Scraping.Middleware.Http;
using HtmlAgilityPack;
using PuppeteerSharp;
using System.Text;

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
			if (context.RequestHandeled) // if request already has been handeled
				return true;

			var browser = await _browserPool.GetBrowserAsync();
			
			try
			{
				var page = await browser.NewPageAsync();
				await page.GoToAsync(context.ScrapeJob.Uri.ToString(), WaitUntilNavigation.Networkidle2);
				var dom = await page.GetContentAsync();
				var result = new HttpResponseData(new MemoryStream(Encoding.UTF8.GetBytes(dom)), "text/html");
				context.AddRequestResult(result);
				return true;
			}
			finally
			{
				await _browserPool.ReturnBrowserAsync(browser);
			}
		}
	}
}
