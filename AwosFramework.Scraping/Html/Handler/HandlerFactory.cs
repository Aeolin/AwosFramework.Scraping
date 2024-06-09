using AwosFramework.Scraping.Utils;
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html.Handler
{
	public class HandlerFactory
	{
		public static IPropertyHandler GetHandler(Type type, IHtmlSelector selector, IHtmlSelector childSelector = null)
		{
			if (type.IsAssignableTo(typeof(HtmlNode)))
			{
				return new HtmlNodeHandler(selector);
			}
			else if (Nullable.GetUnderlyingType(type)?.HasTryParseFunction() ?? false)
			{
				return new NullableHandler(selector, type);
			}
			else if (type.HasParseFunction())
			{
				return new ParseableHandler(selector, type);
			}
			else if (type.IsArray)
			{
				return new ArrayHandler(selector, childSelector, type);
			}
			else if (type.IsAssignableTo(typeof(IEnumerable)) || type.IsAssignableTo(typeof(IList)))
			{
				return new EnumerableHandler(selector, childSelector, type);
			}
			else
			{
				return new ObjectHandler(selector, type);
			}
		}
	}
}
