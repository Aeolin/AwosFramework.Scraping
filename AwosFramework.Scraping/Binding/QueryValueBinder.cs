using AwosFramework.Scraping.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Binding
{
	public class QueryValueBinder : IBinder
	{
		public string ParameterName { get; init; }
		public Type ParameterType { get; init; }
		public object DefaultValue { get; init; }
		private Func<string, object> _conversionFunc;


		public QueryValueBinder(string parameterName, Type parameterType, object defaultValue = null)
		{
			ParameterName=parameterName;
			ParameterType=parameterType;
			_conversionFunc = parameterType.GetParseFunction();
			DefaultValue=defaultValue;
		}

		public object Bind(ScrapingContext context)
		{
			if (context.QueryData.TryGetValue(ParameterName, out var data))
				return _conversionFunc(data);

			return DefaultValue;
		}
	}
}
