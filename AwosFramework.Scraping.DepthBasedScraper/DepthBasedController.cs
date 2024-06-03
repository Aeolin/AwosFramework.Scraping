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
		private static readonly ConcurrentBag<Uri> _queuedUrls = new ConcurrentBag<Uri>();
		private static int _pageCount = 0;
		private readonly DepthBasedScrapingConfig _config;

		public DepthBasedController(DepthBasedScrapingConfig config)
		{
			_config=config;
		}

		[DefaultRoute]
		public async Task<IScrapeResult> ScrapePageAsync([FromJob] int depth = 0)
		{
			_queuedUrls.Add(Url);
			if (this.Content == null)
				return Ok();

			var result = new ScrapedPage { Depth = depth, Url = this.Url.ToString(), Html = this.Content.Text };
			if (depth >= _config.MaxDepth)
				return Ok(result);

			if (Interlocked.Increment(ref _pageCount) > _config.MaxPages)
				return Ok(result);

			var links = this.Content.DocumentNode.SelectNodes("//a[@href]")?
				.Select(link => link.GetAttributeValue("href", null))
				.Select(x => new Uri(this.Url, x))
				.Where(link => _queuedUrls.ContainsAndAdd(link) == false && link.Scheme.StartsWith("http"))
				.Select(link => HttpJob.Get(link, data: depth+1));

			return OkFollow(links, result);
		}
	}
}
