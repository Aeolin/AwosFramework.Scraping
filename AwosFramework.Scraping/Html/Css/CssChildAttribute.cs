using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html.Css
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
	public class CssChildAttribute : CssAttribute
	{
		public CssChildAttribute(string selector) : base(selector)
		{
		}
	}
}
