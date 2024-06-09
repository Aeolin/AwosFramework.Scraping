using AwosFramework.Scraping.Binding.Attributes;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Core.Results;
using AwosFramework.Scraping.Routing;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.AO3
{
	public class AO3Scraper : ScrapeController
	{
		[Route("https://archiveofourown.org/works/search")]
		public IScrapeResult ScrapeWorks([FromXPath("//a[@rel='next']", Attribute = "href")]string next, [FromCss("ol.work li .heading a:first-child", Attribute = "href")]IEnumerable<string> works)
		{
			works = works.Where(x => x.StartsWith("/works")).Select(x => $"https://archiveofourown.org{x}?view_adult=true");
			var jobs = works.Select(HttpJob.Get).ToList();
			if(next != null)
				jobs.Add(HttpJob.Get($"https://archiveofourown.org{next}", 10));

			return Follow(jobs);
		}

		[Route("https://archiveofourown.org/works/{id}")]
		public IScrapeResult ScrapeWorkAsync([FromRoute]int id, [FromXPath("/")]WorkMeta meta)
		{
			meta.Id = id;
			return Ok(meta);
		}
	}
}
