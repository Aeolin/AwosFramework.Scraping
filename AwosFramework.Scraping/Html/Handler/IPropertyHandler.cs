using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html.Handler
{
	public interface IPropertyHandler
	{
		public object Deserialize(HtmlNode root);
		public IHtmlSelector Selector { get; }
	}
}
