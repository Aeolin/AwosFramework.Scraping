using AwosFramework.Scraping.Utils;
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html
{
	public class HtmlDeserializer
	{
		public static T Deserialize<T>(HtmlNode node) => (T)Deserialize(node, typeof(T));
		private static readonly ConcurrentDictionary<Type, HtmlDeserializerTypeMetadata> _typeMetas = new ConcurrentDictionary<Type, HtmlDeserializerTypeMetadata>();

		public static object Deserialize(HtmlNode node, Type type)
		{
			var instance = Activator.CreateInstance(type);
			if (instance == null)
				return null;

			if (_typeMetas.TryGetValue(type, out var meta) == false)
			{
				meta = new HtmlDeserializerTypeMetadata(type);
				_typeMetas.TryAdd(type, meta);
			}

			foreach (var (property, handler) in meta.Properties)
				property.SetValue(instance, handler.Deserialize(node));

			return instance;
		}

	}
}
