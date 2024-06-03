using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Hosting;
using AwosFramework.Scraping.Hosting.Builders;
using AwosFramework.Scraping.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Cli
{
	public class ScrapeApplication : IHost, IScrapeApplicationBuilder
	{
		public static ScrapeHostApplicationBuilder CreateBuilder(string[] args)
		{
			var builder = new ScrapeHostApplicationBuilder();
			builder.Configuration.AddJsonFile("appsettings.json");
			builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
			builder.Logging.AddConsole();
			return builder;
		}

		public IServiceProvider Services { get; init; }
		public MiddlewareCollectionFactory Middleware { get; init; }
		public ResultHandlerCollectionFactory ResultHandlers { get; init; }
		private static Scraper _scraper;
		private readonly List<HttpJob> _initialJobs = new List<HttpJob>();


		internal ScrapeApplication(IServiceProvider provider, MiddlewareCollectionFactory middlewareBuilder, ResultHandlerCollectionFactory resultHandlers)
		{
			Services = provider;
			Middleware = middlewareBuilder;
			ResultHandlers=resultHandlers;
			_scraper = Services.GetRequiredService<Scraper>();
		}

		public IScrapeApplicationBuilder UseMiddleware(Func<IServiceProvider, IMiddleware> middleware)
		{
			Middleware.AddMiddleware(middleware);
			return this;
		}

		public ScrapeApplication AddJobs(params HttpJob[] jobs)
		{
			_initialJobs.AddRange(jobs);
			return this;
		}

		public ScrapeApplication AddJobs(IEnumerable<HttpJob> jobs)
		{
			_initialJobs.AddRange(jobs);
			return this;
		}

		public void Dispose()
		{
			_scope.Dispose();
			_scraper?.Dispose();
		}

		public Task StartAsync(CancellationToken cancellationToken = default)
		{
			return _scraper.RunAsync(_initialJobs.ToArray());
		}

		public Task StopAsync(CancellationToken cancellationToken = default)
		{
			_scraper.Dispose();
			return Task.CompletedTask;
		}

		public IScrapeApplicationBuilder UseResultHandler(object resultHandler)
		{
			_scraper.WithResultHandler(resultHandler);
			return this;
		}
	}
}
