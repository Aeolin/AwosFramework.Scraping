﻿using AwosFramework.Scraping.Binding.Attributes;
using AwosFramework.Scraping.Html;
using AwosFramework.Scraping.Html.Css;
using AwosFramework.Scraping.Html.XPath;
using AwosFramework.Scraping.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Binding.DefaultBinders
{
	public class HtmlBinderGenerator : IBinderGenerator
	{
		private static IHtmlSelector GetSelector(CssAttribute css, XPathAttribute xpath)
		{
			if (xpath != null && css != null)
				throw new InvalidOperationException($"Property can only have {nameof(XPathAttribute)} or {nameof(CssAttribute)} not both");

			if (xpath != null)
				return new XPathSelector(xpath.XPath, xpath.Attribute, xpath.DeserializationType);

			if (css != null)
				return new CssSelector(css.Selector, css.Attribute, css.DeserializationType);

			return null;
		}

		public bool TryCreateBinder(ParameterInfo parameter, RouteMatcher matcher, object defaultValue, out IBinder binder)
		{
			var xpath = parameter.GetCustomAttribute<FromXPathAttribute>();
			var css = parameter.GetCustomAttribute<FromCssAttribute>();
			var selector = GetSelector(css, xpath);
			if (selector != null)
			{
				var childXpath = parameter.GetCustomAttribute<XPathChildAttribute>();
				var childCss = parameter.GetCustomAttribute<CssChildAttribute>();
				var childSelector = GetSelector(childCss, childXpath);
				binder =  new HtmlBinder(parameter.Name, parameter.ParameterType, selector, childSelector, defaultValue);
				return true;
			}
			else
			{
				binder = null;
				return false;
			}

		}
	}
}
