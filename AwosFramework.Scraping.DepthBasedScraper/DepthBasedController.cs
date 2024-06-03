using AwosFramework.Scraping.Binding.Attributes;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Core.Results;
using AwosFramework.Scraping.Routing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.DepthBasedScraper
{
	public class DepthBasedController : ScrapeController
	{
		private static readonly ConcurrentBag<string> _scrapedPages = new ConcurrentBag<string>();
		private static int _pageCount = 0;
		private readonly DepthBasedScrapingConfig _config;

		public DepthBasedController(DepthBasedScrapingConfig config)
		{
			_config=config;
		}

		[DefaultRoute]
		public async Task<IScrapeResult> ScrapePageAsync([FromJob] int depth = 0)
		{
			if (depth > _config.MaxDepth)
				return Ok();

			if (Interlocked.Increment(ref _pageCount) > _config.MaxPages)
				return Ok();

			if (this.Content == null)
				return Ok();

			var links = this.Content.DocumentNode.SelectNodes("//a[@href]")
				.Select(n => n.Attributes["href"].Value)
				.Where(l => !string.IsNullOrWhiteSpace(l) && _scrapedPages.Contains(l) == false)
				.Select(x => HttpJob.Get(x, data: depth+1));

			return OkFollow(links, new ScrapedPage { Depth = depth, Url = this.Url.ToString(), Html = this.Content.Text });
		}
	}
}
