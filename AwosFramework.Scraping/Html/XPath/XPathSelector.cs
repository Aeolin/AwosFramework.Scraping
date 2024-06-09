using AwosFramework.Scraping.Html.PostProcessing;
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
		private IPostProcessor[] _postProcessors;

		public XPathSelector(string xPath, IPostProcessor[] postProcessors, string attribute = null, DeserializationType deserializationType = default)
		{
			_postProcessors = postProcessors;
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


			var value = Attribute != null ? child.GetAttributeValue(Attribute, null) : child.InnerText;
			if (_postProcessors !=null)
				foreach (var postProcessor in _postProcessors)
					value = postProcessor.PostProcess(value);

			return value;
		}
	}
}
