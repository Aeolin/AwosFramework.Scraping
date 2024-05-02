﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AwosFramework.Scraping.Html.Handler
{
	public class ArrayHandler : AbstractPropertyHandler
	{
		private IPropertyHandler _componentHandler;
		private Type _componentType;
		public bool HandlesCollections => true;

		public ArrayHandler(IHtmlSelector selector, IHtmlSelector childSelector, Type type) : base(selector)
		{
			_componentType = type.GetElementType();
			childSelector ??= new SameNodeSelector(selector.Attribute);
			_componentHandler = HandlerFactory.GetHandler(type, childSelector);
		}

		public override object Deserialize(HtmlNode root)
		{
			var nodes = Selector.SelectNodes(root)?.ToList();
			if (nodes == null)
				return null;

			var result = Array.CreateInstance(_componentType, nodes.Count);
			for (int i = 0; i < nodes.Count; i++)
			{
				var value = _componentHandler.Deserialize(nodes[i]);
				result.SetValue(value, i);
			}

			return result;
		}
	}
}
