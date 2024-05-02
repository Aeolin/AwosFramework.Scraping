using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html.Handler
{
	public abstract class AbstractPropertyHandler : IPropertyHandler
	{
		public IHtmlSelector Selector { get; init; }

		public AbstractPropertyHandler(IHtmlSelector selector)
		{
			Selector=selector;
		}

		public abstract object Deserialize(HtmlNode root);
	}
}
