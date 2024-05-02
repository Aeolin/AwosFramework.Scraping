using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html.XPath
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
	public class XPathChildAttribute : XPathAttribute
	{
		public XPathChildAttribute(string xPath) : base(xPath)
		{
		}
	}
}
