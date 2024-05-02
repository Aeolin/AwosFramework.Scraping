using AwosFramework.Scraping.Routing;
using System.Reflection;

namespace AwosFramework.Scraping.Binding
{
	public interface IBinderFactory
	{
		IBinder CreateBinder(ParameterInfo parameter, RouteMatcher matcher, object defaultValue);
	}
}