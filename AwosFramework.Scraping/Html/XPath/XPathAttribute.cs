using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html.XPath
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class XPathAttribute : Attribute
	{
		public string XPath { get; set; }
		public string Attribute { get; init; }
		public DeserializationType DeserializationType { get; init; }


		public XPathAttribute(string xPath)
		{
			XPath = xPath;
		}
	}
}
