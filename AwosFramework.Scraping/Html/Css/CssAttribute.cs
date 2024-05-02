using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html.Css
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class CssAttribute : Attribute
	{
		public string Selector { get; init; }
		public string Attribute { get; init; }
		public DeserializationType DeserializationType { get; init; }

		public CssAttribute(string selector)
		{
			Selector = selector;
		}
	}
}
