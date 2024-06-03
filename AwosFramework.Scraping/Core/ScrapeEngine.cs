using AwosFramework.Scraping.Core.Results;
using AwosFramework.Scraping.Middleware;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AwosFramework.Scraping.Core
{
	public class ScrapeEngine
	{
		private IServiceProvider _provider;
		private ILogger _logger;
		private readonly MiddlewareCollection _middleware;

		public bool IsScraping { get; private set; } = false;

		public ScrapeEngine(IServiceProvider provider, ILoggerFactory factory, MiddlewareCollection middleware)
		{
			_logger = factory.CreateLogger<ScrapeEngine>();
			_provider = provider;
			_middleware = middleware;
		}

		public ScrapeEngine(IServiceProvider container, ILoggerFactory factory, RouteMap router)
		{
			_provider = container;
			var client = container.GetService<HttpClient>();
			if (client == null)
				throw new InvalidOperationException($"Couldn't get an http client from service provider");

			_logger = factory.CreateLogger<ScrapeEngine>();
		}

		public async Task<IScrapeResult> ScrapeAsync(IScrapeJob job)
		{
			IsScraping = true;
			var scope = _provider.CreateScope();
			var loggerScope = _logger.BeginScope("ScrapeJob[{0}]", job.Id);
			var context = new MiddlewareContext(job, scope.ServiceProvider, _logger);
			try
			{
				foreach (var middleware in _middleware)
				{
					var result = await middleware.ExecuteAsync(context);
					if(result == false)
					{
						_logger.LogWarning("Middleware {0} didn't execute successfully", middleware.GetType().Name);
						return new FailedResult($"Middleware {middleware.GetType().Name} didn't execute successfully");
					}
				}

				return context.GetRequiredComponent<IScrapeResult>();
			}
			catch (Exception ex)
			{
				return new FailedResult(ex);
			}
			finally
			{
				loggerScope.Dispose();
				scope.Dispose();
				context.Dispose();
				IsScraping = false;
			}
		}
	}
}
