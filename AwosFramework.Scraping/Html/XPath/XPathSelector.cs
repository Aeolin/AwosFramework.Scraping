using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html.XPath
{
	public class XPathSelector : IHtmlSelector
	{
		public string XPath { get; init; }
		public string Attribute { get; init; }
		public DeserializationType DeserializationType { get; init; }

		public XPathSelector(string xPath, string attribute = null, DeserializationType deserializationType = default)
		{
			XPath = xPath;
			Attribute=attribute;
			DeserializationType=deserializationType;
		}

		public IEnumerable<HtmlNode> SelectNodes(HtmlNode node) => node.SelectNodes(XPath);
		public HtmlNode SelectSingleNode(HtmlNode node) => node.SelectSingleNode(XPath);

		public string SelectNodeValue(HtmlNode node)
		{
			var child = SelectSingleNode(node);
			if (child == null)
				return null;

			return Attribute != null ? child.GetAttributeValue(Attribute, null) : child.InnerText;
		}
	}
}
