using AwosFramework.Scraping.Html.Css;
using AwosFramework.Scraping.Html.XPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Reddit
{
	public class Comment
	{
		[XPath(Attribute = "id")]
		public required string Id { get; set; }

		[Css(".usertext-body")]
		public required string Content { get; set; }

		[XPath(Attribute = "data-author-fullname")]
		public required string AuthorId { get; set; }

		[XPath("ancestor::div[contains(@class, 'comment')][position() = 1]", Attribute = "id")]
		public string? ReferencedCommentId { get; set; }

		[Css("> .entry .tagline .score.unvoted", Attribute = "title")]
		public int? Score { get; set; }
	}
}
