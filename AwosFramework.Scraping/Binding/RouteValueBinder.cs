using AwosFramework.Scraping.Middleware;
using AwosFramework.Scraping.Middleware.Routing;
using AwosFramework.Scraping.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Binding
{
	public class RouteValueBinder : IBinder
	{
		public string ParameterName { get; init; }
		public Type ParameterType { get; init; }
		public object DefaultValue { get; init; }
		private Func<string, object> _conversionFunc;

		public RouteValueBinder(string parameterName, Type parameterType, object defaultValue = null)
		{
			ParameterName = parameterName;
			ParameterType = parameterType;
			DefaultValue = defaultValue;
			_conversionFunc = parameterType.GetParseFunction();
		}

		public object Bind(ScrapingContext context)
		{
			if(context.RouteData.TryGetValue(ParameterName, out var data))	
				return _conversionFunc(data);
			
			return DefaultValue;
		}

		public object Bind(MiddlewareContext context)
		{
			var data = context.GetRequiredComponent<RouteData>();
			if (data.TryGetValue(ParameterName, out var value))
				return _conversionFunc(value);

			return DefaultValue;
		}
	}
}
