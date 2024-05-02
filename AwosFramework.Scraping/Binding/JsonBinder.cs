using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Binding
{
	public class JsonBinder : IBinder
	{
		public string ParameterName { get; init; }
		public Type ParameterType { get; init; }
		public object DefaultValue { get; init; }

		public JsonBinder(string parameterName, Type parameterType, object defaultValue)
		{
			ParameterName=parameterName;
			ParameterType=parameterType;
			DefaultValue=defaultValue;
		}

		public object Bind(ScrapingContext context)
		{
			if (context.JsonContent == null)
				return DefaultValue;

			return JsonSerializer.Deserialize(context.JsonContent, ParameterType);
		}
	}
}
