using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html
{
	public interface IHtmlSelector
	{
		public string Attribute { get; }
		public bool HasAttribute => Attribute != null;
		public DeserializationType DeserializationType { get; init; }
		public HtmlNode SelectSingleNode(HtmlNode node);
		public string SelectNodeValue(HtmlNode node);
		public IEnumerable<HtmlNode> SelectNodes(HtmlNode node);
	}
}
