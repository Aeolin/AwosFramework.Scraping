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
		private static readonly ConcurrentDictionary<string, string> _queuedUrls = new ConcurrentDictionary<string, string>(-1, 1000000);
		private static int _pageCount = 0;
		private readonly DepthBasedScrapingConfig _config;
		private static readonly UriCreationOptions UriOpts = new UriCreationOptions();

		public DepthBasedController(DepthBasedScrapingConfig config)
		{
			_config=config;
		}

		[DefaultRoute]
		public async Task<IScrapeResult> ScrapePageAsync([FromJob] DepthData data)
		{
			_queuedUrls.TryAdd(Url.GetLeftPart(UriPartial.Query), null);
			if (this.Content == null)
				return Ok();

			var result = new ScrapedPage { Depth = data.Depth, Url = this.Url.ToString(), Html = this.Content.Text };
			if (data.Depth >= _config.MaxDepth)
					return Ok(result);

			int prio = 0;
			var links = this.Content.DocumentNode.SelectNodes("//a[@href]")?
				.Select(link => link.GetAttributeValue("href", null))
				.SelectWhere(x => (Uri.TryCreate(data.BaseUri, x, out var uri), uri))
				.Where(link =>
				{
					var lastSegment = link.Segments.LastOrDefault();
					return data.BaseUri.IsBaseOf(link) &&
					link.Scheme.StartsWith("http") &&	
					(lastSegment == null || lastSegment.Contains('.') == false || lastSegment.EndsWith(".html", StringComparison.OrdinalIgnoreCase)) &&
					_queuedUrls.TryAdd(link.GetLeftPart(UriPartial.Query), null) == false;
				})
				.Select(link => HttpJob.Get(link, prio++, data with { Depth = data.Depth+1 }));

			return OkFollow(links, result);
		}
	}
}
