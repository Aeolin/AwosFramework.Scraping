using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html.Handler
{
	public class StringHandler : AbstractPropertyHandler
	{
		public StringHandler(IHtmlSelector selector) : base(selector)
		{
		}

		public override object Deserialize(HtmlNode node) =>  Selector.SelectNodeValue(node);

	}
}
