using AwosFramework.Scraping.Binding.Attributes;
using AwosFramework.Scraping.Html;
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
			var fromBody = parameter.GetCustomAttribute<FromBodyAttribute>();
			if (fromBody == null)
			{
				binder = null;
				return false;
			}

			switch (fromBody.DeserializationType)
			{
				case DeserializationType.Json:
					binder = new JsonBinder(parameter.Name, parameter.ParameterType, defaultValue);
					return true;

				case DeserializationType.Html:
					binder = new HtmlBinder(parameter.Name, parameter.ParameterType, new SameNodeSelector(), null);
					return true;

				default:
					throw new ArgumentException($"Unsupported deserialization type: {fromBody.DeserializationType}. Supported types are: Json, Html.", nameof(fromBody.DeserializationType));
			}

		}
	}
}
