using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html.Handler
{
	public class HtmlNodeHandler : AbstractPropertyHandler
	{
		public HtmlNodeHandler(IHtmlSelector selector) : base(selector)
		{
		}

		public override object Deserialize(HtmlNode root) => Selector.SelectSingleNode(root);
	}
}
