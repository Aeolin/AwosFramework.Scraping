using AwosFramework.Scraping.Html.PostProcessing;
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
		private IPostProcessor[] _postProcessors;


		public CssSelector(string selector, IPostProcessor[] postProcessors, string attribute = null, DeserializationType deserializationType = default)
		{
			_postProcessors = postProcessors;
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

			var value = Attribute != null ? child.GetAttributeValue(Attribute, null) : child.InnerText;
			if (_postProcessors !=null)
				foreach (var postProcessor in _postProcessors)
					value = postProcessor.PostProcess(value);

			return value;
		}
	}
}
