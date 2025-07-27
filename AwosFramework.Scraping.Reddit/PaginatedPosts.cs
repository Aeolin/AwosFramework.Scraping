using AwosFramework.Scraping.Html.Css;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Reddit
{
	public class PaginatedPosts
	{
		[Css("#siteTable .thing:not(.stickied) .buttons a.comments", Attribute = "href")]
		public required string[] PostUrls { get; set; }

		[Css(".next-button a", Attribute = "href")]
		public required string NextUrl { get; set; }
	}
}
