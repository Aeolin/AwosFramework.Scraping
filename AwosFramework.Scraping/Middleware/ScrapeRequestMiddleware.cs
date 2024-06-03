using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware
{
	public class ScrapeRequestMiddleware : IMiddleware
	{
		public async Task<bool> ExecuteAsync(MiddlewareContext context)
		{
			var handler = context.GetRequiredComponent<IScrapeRequestHandler>();
			var result = await handler.HandleAsync(context);
			context.AddComponent(result);
			return true;
		}
	}
}
