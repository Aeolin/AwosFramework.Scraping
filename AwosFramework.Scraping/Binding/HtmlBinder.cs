using AwosFramework.Scraping.Html;
using AwosFramework.Scraping.Html.Handler;
using AwosFramework.Scraping.Middleware;
using AwosFramework.Scraping.Utils;
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AwosFramework.Scraping.Binding
{
	public class HtmlBinder : IBinder
	{
		public string ParameterName { get; init; }
		public Type ParameterType { get; init; }
		public object DefaultValue { get; init; }
		private IPropertyHandler _handler;


		public HtmlBinder(string parameterName, Type parameterType, IHtmlSelector selector, IHtmlSelector childSelector, object defaultValue = null)
		{
			ParameterName=parameterName;
			ParameterType=parameterType;
			_handler = HandlerFactory.GetHandler(parameterType, selector, childSelector);
			DefaultValue=defaultValue;
		}

		public object Bind(ScrapingContext context)
		{
			if (context.HtmlContent == null)
				return DefaultValue;

			return _handler.Deserialize(context.HtmlContent.DocumentNode);
		}

		public object Bind(MiddlewareContext context)
		{
			var html = context.GetRequiredComponent<HtmlDocument>();
			return _handler.Deserialize(html.DocumentNode);
		}
	}
}
