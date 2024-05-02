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
	public class RouteBinderGenerator : IBinderGenerator
	{
		public bool TryCreateBinder(ParameterInfo parameter, RouteMatcher matcher, object defaultValue, out IBinder binder)
		{
			var query = parameter.GetCustomAttribute<FromRouteAttribute>();
			if (query != null)
			{
				binder = new RouteValueBinder(query.Key ?? parameter.Name, parameter.ParameterType, defaultValue);
				return true;
			}

			binder = null;
			return false;
		}
	}
}
