using AwosFramework.Scraping.Html.Css;
using AwosFramework.Scraping.Html.PostProcessing;
using AwosFramework.Scraping.Html.XPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Reddit
{
	public class Post
	{
		[Css("#siteTable > .thing", Attribute = "data-subreddit")]
		public required string Subreddit { get; set; }

		[Css("#siteTable > .thing", Attribute = "data-fullname")]
		public required string Id { get; set; }

		[Css("#siteTable > .thing", Attribute = "data-permalink")]
		public required string Url { get; set; }

		[Trim]
		[Css("#siteTable a.title")]
		public required string Title { get; set; }

		[Css(".commentarea .comment:not(.stickied)")]
		public required Comment[] Comments { get; init; } 
	}
}
