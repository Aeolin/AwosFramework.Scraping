using AwosFramework.Scraping.Core.Results;
using AwosFramework.Scraping.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting
{
	public class AnonymousScrapeRequestHandler : IScrapeRequestHandler
	{
		private readonly Func<MiddlewareContext, Task<IScrapeResult>> _function;

		public AnonymousScrapeRequestHandler(Func<MiddlewareContext, Task<IScrapeResult>> function)
		{
			_function=function;
		}

		public Task<IScrapeResult> HandleAsync(MiddlewareContext context)
		{
			return _function(context);
		}
	}
}
