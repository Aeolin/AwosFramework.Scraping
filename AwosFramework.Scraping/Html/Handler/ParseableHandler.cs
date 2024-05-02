using AwosFramework.Scraping.Utils;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AwosFramework.Scraping.Html.Handler
{
	public class ParseableHandler : AbstractPropertyHandler
	{
		private Func<string, object> _parseFunc;
		public bool HandlesCollections => false;


		public ParseableHandler(IHtmlSelector selector, Type type) : base(selector)
		{
			_parseFunc = type.GetParseFunction();
		}

		public override object Deserialize(HtmlNode root)
		{
			var value = Selector.SelectNodeValue(root);
			return _parseFunc(value);
		}
	}
}
