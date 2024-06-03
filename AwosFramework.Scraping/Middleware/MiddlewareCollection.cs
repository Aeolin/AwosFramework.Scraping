using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware
{
	public class MiddlewareCollection : ReadOnlyCollection<IMiddleware>
	{
		public MiddlewareCollection(IList<IMiddleware> list) : base(list)
		{
		}
	}
}
