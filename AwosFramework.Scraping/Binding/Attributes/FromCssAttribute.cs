using AwosFramework.Scraping.Html.Css;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Binding.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class FromCssAttribute : CssAttribute
	{
		public FromCssAttribute(string selector) : base(selector)
		{
		}
	}
}
