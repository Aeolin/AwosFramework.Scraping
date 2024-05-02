using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Binding
{
	public class JobBinder : IBinder
	{
		public string ParameterName {get;init;}
		public Type ParameterType { get; init; }
		public object DefaultValue { get; init; }

		public JobBinder(string parameterName, Type parameterType, object defaultValue = null)
		{
			ParameterName=parameterName;
			ParameterType=parameterType;
			DefaultValue=defaultValue;
		}

		public object Bind(ScrapingContext context)
		{
			if(context.JobData != null && context.JobData.GetType().IsAssignableTo(ParameterType))
				return context.JobData;

			return DefaultValue;
		}
	}
}
