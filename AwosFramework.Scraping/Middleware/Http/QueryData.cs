using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware.Http
{
	public class QueryData : ReadOnlyDictionary<string, string>
	{
		public QueryData(IDictionary<string, string> dictionary) : base(dictionary)
		{
		}
	}
}
