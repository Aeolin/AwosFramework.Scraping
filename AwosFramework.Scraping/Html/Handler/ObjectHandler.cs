using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace AwosFramework.Scraping.Html.Handler
{
	public class ObjectHandler : AbstractPropertyHandler
	{
		private Type _type;

		public ObjectHandler(IHtmlSelector selector, Type type) : base(selector)
		{
			_type = type;
		}

		public override object Deserialize(HtmlNode root)
		{
			var type = Selector.DeserializationType;
			if (type == DeserializationType.Auto)
				type = Selector.HasAttribute ? DeserializationType.Json : DeserializationType.Html;

			switch (type)
			{
				case DeserializationType.Json:
					return DeserializeJson(root);
				case DeserializationType.Html:
					return DeserializeHtml(root);
				default:
					throw new InvalidOperationException("Invalid deserialization type");
			}
		}

		private object DeserializeHtml(HtmlNode root)
		{
			HtmlNode node;
			if (Selector.HasAttribute)
			{
				var doc = new HtmlDocument();
				doc.Load(Selector.SelectNodeValue(root));
				node = doc.DocumentNode;
			}
			else
			{
				node = Selector.SelectSingleNode(root);
			}

			return HtmlDeserializer.Deserialize(node, _type);
		}

		private object DeserializeJson(HtmlNode root)
		{
			var value = Selector.SelectNodeValue(root);
			value = HttpUtility.HtmlDecode(value);
			return JsonSerializer.Deserialize(value, _type);
		}
	}
}
