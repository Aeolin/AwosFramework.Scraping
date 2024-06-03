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
		private readonly IScrapeRequestHandler _handler;

		public DefaultRouteMiddleware(IScrapeRequestHandler handler)
		{
			_handler=handler;
		}

		public async Task<bool> ExecuteAsync(MiddlewareContext context)
		{
			if(context.HasComponent<IScrapeResult>() == false)
			{
				var result = await _handler.HandleAsync(context);
				context.AddComponent(result);
			}

			return true;
		}
	}
}
