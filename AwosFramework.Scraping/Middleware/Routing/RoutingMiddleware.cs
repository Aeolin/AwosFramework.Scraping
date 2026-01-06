using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware.Routing
{
	public class RoutingMiddleware : IMiddleware
	{
		private RouteMap _routeMap;

		public RoutingMiddleware(RouteMap routeMap)
		{
			_routeMap = routeMap;
		}

		public Task<bool> ExecuteAsync(MiddlewareContext context)
		{
			if (context.ScrapeJob.HandlerName != null)
			{
				if (_routeMap.TryGetNamedHandler(context.ScrapeJob.HandlerName, out var method) == false)
					return Task.FromResult(false);

				context.AddComponent<IScrapeDataHandler>(method);
				return Task.FromResult(true);
			}
			else if (_routeMap.TryRoute(context.ScrapeJob.Uri, out var method, out var routeMatch))
			{
				context.AddComponent<IScrapeDataHandler>(method);
				context.AddComponent(new RouteData(routeMatch.Data));

				return Task.FromResult(true);
			}

			return Task.FromResult(false);
		}
	}
}
