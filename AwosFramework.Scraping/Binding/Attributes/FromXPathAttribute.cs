using AwosFramework.Scraping.Html.XPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Binding.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class FromXPathAttribute : XPathAttribute
	{
		public FromXPathAttribute(string xPath) : base(xPath)
		{
		}
	}
}
