using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html.Css
{
	public class CssSelector : IHtmlSelector
	{
		public string Selector { get; init; }
		public string Attribute { get; init; }
		public DeserializationType DeserializationType { get; init; }

		public CssSelector(string selector, string attribute = null, DeserializationType deserializationType = default)
		{
			Selector = selector;
			Attribute=attribute;
			DeserializationType=deserializationType;
		}

		public IEnumerable<HtmlNode> SelectNodes(HtmlNode node) => node.QuerySelectorAll(Selector);
		public HtmlNode SelectSingleNode(HtmlNode node) => node.QuerySelector(Selector);

		public string SelectNodeValue(HtmlNode node)
		{
			var child = SelectSingleNode(node);
			if (child == null)
				return null;

			return Attribute != null ? child.GetAttributeValue(Attribute, null) : child.InnerText;
		}
	}
}
