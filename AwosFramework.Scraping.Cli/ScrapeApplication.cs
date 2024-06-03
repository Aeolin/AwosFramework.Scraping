using AwosFramework.Scraping.Core;
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
	public class ScrapeApplication : IHost
	{
		public static ScrapeApplicationBuilder CreateBuilder(string[] args)
		{
			var builder = new ScrapeApplicationBuilder();
			builder.Configuration.AddJsonFile("appsettings.json");
			builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
			builder.Logging.AddConsole();
			return builder;
		}

		public IServiceProvider Services { get; init; }
		private static Scraper _scraper;
		private readonly List<ScrapeJob> _initialJobs = new List<ScrapeJob>();

		internal ScrapeApplication(IServiceProvider provider)
		{
			Services = provider;
			_scraper = provider.GetRequiredService<Scraper>();
		}

		public ScrapeApplication AddJobs(params ScrapeJob[] jobs)
		{
			_initialJobs.AddRange(jobs);
			return this;
		}

		public ScrapeApplication AddJobs(IEnumerable<ScrapeJob> jobs)
		{
			_initialJobs.AddRange(jobs);
			return this;
		}

		public void Dispose()
		{
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
	}
}
