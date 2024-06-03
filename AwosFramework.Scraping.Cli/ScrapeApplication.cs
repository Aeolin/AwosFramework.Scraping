using AwosFramework.Scraping.Core;
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

		public IServiceProvider Services => _scope.ServiceProvider;
		private IServiceScope _scope;
		private static Scraper _scraper;
		private readonly List<HttpJob> _initialJobs = new List<HttpJob>();

		private readonly List<Func<IServiceProvider, IMiddleware>> _middleware = new List<Func<IServiceProvider, IMiddleware>>();


		internal ScrapeApplication(IServiceProvider provider)
		{
			_scope = provider.CreateScope();
			_scraper = Services.GetRequiredService<Scraper>();
		}

		public IScrapeApplicationBuilder AddMiddleware(Func<IServiceProvider, IMiddleware> middleware)
		{
			_middleware.Add(middleware);
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

		public IScrapeApplicationBuilder UseMiddleware(object middleware)
		{
			throw new NotImplementedException();
		}

		public IScrapeApplicationBuilder UseResultHandler(object resultHandler)
		{
			_scraper.WithResultHandler(resultHandler);
			return this;
		}
	}
}
