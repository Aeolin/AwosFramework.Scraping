using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware.Routing
{
	public class RouteData : ReadOnlyDictionary<string, string>
	{
		public RouteData(IDictionary<string, string> dictionary) : base(dictionary)
		{
		}
	}
}
