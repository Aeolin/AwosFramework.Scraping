using AwosFramework.Scraping.Binding.Attributes;
using AwosFramework.Scraping.Html.Css;
using AwosFramework.Scraping.Html.PostProcessing;
using AwosFramework.Scraping.Html.XPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.AO3
{
	public class WorkMeta
	{
		public int Id { get; set; }

		[Trim]
		[Css("h2.title")]
		public string Title { get; set; }

		[Trim]
		[Css("div.summary blockquote")]
		public string Summary { get; set; }

		[XPath("//a[@rel='author']", Attribute = "href")]
		public string Author { get; set; }

		[XPath("//dd[@class='language']", Attribute = "lang")]	
		public string Language { get; set; }

		[Css("dd.rating a", Attribute = "href")]
		public string Rating { get; set; }

		[Css("dd.warning a", Attribute = "href")]
		public string[] Warnings { get; set; }

		[Css("dd.category a", Attribute = "href")]
		public string[] Categories { get; set; }

		[Css("dd.fandom a", Attribute = "href")]
		public string[] Fandoms { get; set; }

		[Css("dd.relationship a", Attribute = "href")]
		public string[] Relationships { get; set; }

		[Css("dd.character a", Attribute = "href")]
		public string[] Characters { get; set; }

		[Css("dd.freeform a", Attribute = "href")]
		public string[] Freeforms { get; set; }

		[Css("dl.stats dd.published")]
		public DateTime? Published { get; set; }

		[CleanNumber]
		[Css("dl.stats dd.words")]
		public int? Words { get; set; }
		
		[Css("dl.stats dd.chapters")]
		public string Chapter { get; set; }

		[CleanNumber]
		[Css("dl.stats dd.comments")]
		public int? Comments { get; set; }

		[CleanNumber]
		[Css("dl.stats dd.hits")]
		public int? Hits { get; set; }

		[CleanNumber]
		[Css("dl.stats dd .kudos")]
		public int? Kudos { get; set; }

		[Css("#kudos a", Attribute = "href")]
		public string[] UserKudos { get; set; }
	}
}
