using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html.Handler
{
	public class EnumerableHandler : AbstractPropertyHandler
	{
		private IPropertyHandler _componentHandler;
		private Type _componentType;
		public bool HandlesCollections => true;

		public EnumerableHandler(IHtmlSelector selector, IHtmlSelector childSelector, Type type) : base(selector)
		{
			_componentType =  type.GetGenericArguments().First();
			childSelector ??= new SameNodeSelector(selector.Attribute);
			_componentHandler = HandlerFactory.GetHandler(_componentType, childSelector);
		}

		public override object Deserialize(HtmlNode root)
		{
			var nodes = Selector.SelectNodes(root)?.ToList();
			if (nodes == null)
				return null;

			var result = (IList)typeof(List<>).MakeGenericType(_componentType).GetConstructor(Array.Empty<Type>()).Invoke(null);
			foreach(var node in  nodes)
			{
				var value = _componentHandler.Deserialize(node);
				result.Add(value);
			}

			return result;
		}
	}
}
