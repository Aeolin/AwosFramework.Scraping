using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Binding.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class FromRouteAttribute : Attribute
	{
		public string Key { get; init; }

		public FromRouteAttribute(string key = null)
		{
			Key=key;
		}
	}
}
