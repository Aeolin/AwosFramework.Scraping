using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html
{
	public class SameNodeSelector : IHtmlSelector
	{
		public string Attribute { get; init; }
		public DeserializationType DeserializationType { get; init; }

		public SameNodeSelector(string attribute = null)
		{
			Attribute=attribute;
		}

		public IEnumerable<HtmlNode> SelectNodes(HtmlNode node)
		{
			return [node]; 
		}

		public string SelectNodeValue(HtmlNode node)
		{
			return Attribute != null ? node.GetAttributeValue(Attribute, null) : node.InnerText.Trim();
		}

		public HtmlNode SelectSingleNode(HtmlNode node)
		{
			return node;
		}
	}
}
