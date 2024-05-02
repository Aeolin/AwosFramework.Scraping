using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Core.Results;
using AwosFramework.Scraping.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping
{
	public class RouteMap
	{
		private readonly List<ControllerMethod> _routes;

		public RouteMap(IEnumerable<ControllerMethod> routes = null)
		{
			_routes=routes?.ToList() ?? new List<ControllerMethod>();
		}

		public void AddRoute(ControllerMethod route)
		{
			_routes.Add(route);
		}

		public bool TryRoute(Uri uri, out ControllerMethod method, out RouteMatchResult result)
		{
			foreach(var route in _routes)
			{
				method = route;
				result = route.MatchResult(uri);
				if (result.Success)
					return true;
			}

			method = null;
			result = RouteMatchResult.Failed;
			return false;
		}


	}
}
