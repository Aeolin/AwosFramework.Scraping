using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AwosFramework.Scraping.Utils.Extensions;

namespace AwosFramework.Scraping.Html.Handler
{
	public class NullableHandler : AbstractPropertyHandler
	{
		private IPropertyHandler _componentHandler;
		private Type _componentType;
		private TryParseDelegate _tryParse;

		public NullableHandler(IHtmlSelector selector, Type type) : base(selector)
		{
			_componentType = Nullable.GetUnderlyingType(type);
			if (_componentType.HasTryParseFunction() == false)
				throw new ArgumentException("Type does not have a TryParse function");

			_tryParse = _componentType.CreateTryParseDelegate();
		}

		public override object Deserialize(HtmlNode root)
		{
			var result = Selector.SelectNodeValue(root);
			if (_tryParse(result, out object value))
				return value;

			return null;
		}
	}
}
