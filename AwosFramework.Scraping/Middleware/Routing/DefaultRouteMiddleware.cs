using AwosFramework.Scraping.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware.Routing
{
	public class DefaultRouteMiddleware : IMiddleware
	{
		private readonly IScrapeDataHandler _handler;

		public DefaultRouteMiddleware(IScrapeDataHandler handler)
		{
			_handler=handler;
		}

		public async Task<bool> ExecuteAsync(MiddlewareContext context)
		{
			if(context.HasComponent<IScrapeDataHandler>() == false)
			{
				context.AddComponent(_handler);
			}

			return true;
		}
	}
}
