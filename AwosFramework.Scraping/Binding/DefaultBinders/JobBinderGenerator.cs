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
	public class JobBinderGenerator : IBinderGenerator
	{
		public bool TryCreateBinder(ParameterInfo parameter, RouteMatcher matcher, object defaultValue, out IBinder binder)
		{
			var jobAttr = new FromJobAttribute();
			if (parameter.GetCustomAttribute(jobAttr.GetType()) != null)
			{
				binder = new JobBinder(parameter.Name, parameter.ParameterType, defaultValue);
				return true;
			}

			binder = null;
			return false;
		}
	}
}
