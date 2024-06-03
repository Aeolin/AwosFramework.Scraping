using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Hosting;
using AwosFramework.Scraping.Hosting.Builders;
using AwosFramework.Scraping.Middleware;
using AwosFramework.Scraping.ResultHandling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting
{
	public class ScrapeApplication : IHost, IScrapeApplicationBuilder
	{
		public static ScrapeHostApplicationBuilder CreateBuilder(string[] args)
		{
			var builder = new ScrapeHostApplicationBuilder();
			builder.Configuration.AddJsonFile("appsettings.json");
			builder.AddDefaultServices();
			builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
			builder.Logging.AddConsole();
			return builder;
		}

		public IServiceProvider Services { get; init; }
		public MiddlewareCollectionFactory Middleware { get; init; }
		public ResultHandlerCollectionFactory ResultHandlers { get; init; }
		private static Scraper _scraper;
		private readonly List<IScrapeJob> _initialJobs = new List<IScrapeJob>();


		public ScrapeApplication(IServiceProvider provider, MiddlewareCollectionFactory middlewareBuilder, ResultHandlerCollectionFactory resultHandlers)
		{
			Services = provider;
			Middleware = middlewareBuilder;
			ResultHandlers = resultHandlers;
			_scraper = Services.GetRequiredService<Scraper>();
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

		public IScrapeApplicationBuilder AddInitialJobs(params IScrapeJob[] jobs)
		{
			_initialJobs.AddRange(jobs);
			return this;
		}

		public IScrapeApplicationBuilder AddInitialJobs(IEnumerable<IScrapeJob> jobs)
		{
			_initialJobs.AddRange(jobs);
			return this;
		}

		public IScrapeApplicationBuilder UseMiddleware(Func<IServiceProvider, IMiddleware> middleware)
		{
			Middleware.AddMiddleware(middleware);
			return this;
		}

		public IScrapeApplicationBuilder UseResultHandler(Func<IServiceProvider, IResultHandler> resultHandlerFactory)
		{
			ResultHandlers.AddResultHandler(resultHandlerFactory);
			return this;
		}

		public void Dispose()
		{
			_scraper?.Dispose();
		}
	}
}
