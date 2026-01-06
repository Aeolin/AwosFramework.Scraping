using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Routing
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public class HandlerNameAttribute : Attribute
	{
		public string HandlerName { get; init; }
		public HandlerNameAttribute(string handlerName)
		{
			HandlerName = handlerName;
		}
	}
}
