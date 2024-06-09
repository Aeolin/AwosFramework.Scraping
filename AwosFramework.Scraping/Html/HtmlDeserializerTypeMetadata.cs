using AwosFramework.Scraping.Html.Css;
using AwosFramework.Scraping.Html.Handler;
using AwosFramework.Scraping.Html.PostProcessing;
using AwosFramework.Scraping.Html.XPath;
using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html
{
	public class HtmlDeserializerTypeMetadata
	{
		public Type Type { get; init; }
		public FrozenDictionary<PropertyInfo, IPropertyHandler> Properties { get; init; }
		private static ConcurrentDictionary<Type, IPostProcessor> _ppInstances = new ConcurrentDictionary<Type, IPostProcessor>();

		private IHtmlSelector GetSelector(IPostProcessor[] postProcessors, CssAttribute css, XPath.XPathAttribute xpath)
		{
			if (xpath != null && css != null)
				throw new InvalidOperationException($"Property can only have {nameof(XPath.XPathAttribute)} or {nameof(CssAttribute)} not both");

			if (xpath != null)
				return new XPathSelector(xpath.XPath, postProcessors, xpath.Attribute, xpath.DeserializationType);

			if (css != null)
				return new CssSelector(css.Selector, postProcessors, css.Attribute, css.DeserializationType);

			return null;
		}


		public HtmlDeserializerTypeMetadata(Type type)
		{
			Type=type;
			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanWrite);
			var dict = new Dictionary<PropertyInfo, IPropertyHandler>();
			foreach (var property in properties)
			{
				var xPathAttr = property.GetCustomAttribute<XPath.XPathAttribute>();
				var cssAttr = property.GetCustomAttribute<CssAttribute>();
				var postProcessorTypes = property.GetCustomAttributes<PostProcessorAttribute>();
				var postProcessors = postProcessorTypes.Select(x => _ppInstances.GetOrAdd(x.PostProcessor, (IPostProcessor)Activator.CreateInstance(x.PostProcessor))).ToArray();

				IHtmlSelector selector = GetSelector(postProcessors, cssAttr, xPathAttr);
				if (selector == null)
					continue;

				var xPathChildAttr = property.GetCustomAttribute<XPathChildAttribute>();
				var cssChildAttr = property.GetCustomAttribute<CssChildAttribute>();
				IHtmlSelector childSelector = GetSelector(postProcessors, cssChildAttr, xPathChildAttr);
				dict[property] = HandlerFactory.GetHandler(property.PropertyType, selector, childSelector);
			}

			Properties = dict.ToFrozenDictionary();
		}
	}
}
