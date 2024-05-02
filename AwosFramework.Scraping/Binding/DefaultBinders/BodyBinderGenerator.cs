using AwosFramework.Scraping.Binding.Attributes;
using AwosFramework.Scraping.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Binding.DefaultBinders
{
	public class BodyBinderGenerator : IBinderGenerator
	{
		public bool TryCreateBinder(ParameterInfo parameter, RouteMatcher matcher, object defaultValue, out IBinder binder)
		{
			var job = parameter.GetCustomAttribute<FromBodyAttribute>();
			if (job != null)
			{
				binder = new JsonBinder(parameter.Name, parameter.ParameterType, defaultValue);
				return true;
			}

			binder = null;
			return false;
		}
	}
}
