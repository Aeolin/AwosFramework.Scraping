using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.ResultHandling
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class HandleResultAttribute : Attribute
	{
		public ResultMatchType MatchType { get; init; }

		public HandleResultAttribute(ResultMatchType matchType = ResultMatchType.Assignable)
		{
			MatchType=matchType;
		}
	}
}
