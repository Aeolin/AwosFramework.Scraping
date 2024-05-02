using AwosFramework.Scraping.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Binding
{
	public interface IBinderGenerator
	{
		public bool TryCreateBinder(ParameterInfo parameter, RouteMatcher matcher, object defaultValue, out IBinder binder);
	}
}
