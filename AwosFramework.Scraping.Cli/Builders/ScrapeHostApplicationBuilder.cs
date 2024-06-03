using App.Metrics;
using AwosFramework.Scraping.Cli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting.Builders
{
	public class ScrapeHostApplicationBuilder : IHostApplicationBuilder
	{
		public IServiceCollection Services { get; init; } = new ServiceCollection();

		public ScrapeHostApplicationBuilder()
		{
			Services.AddLogging(x => Logging = x);
			Services.AddMetrics(x => Metrics = x);
			Services.AddSingleton<IOptions<ScraperConfiguration>>();
			Services.Configure<ScraperConfiguration>(Configuration.GetSection("Scraper"));
			Services.AddScoped(x => x.GetRequiredService<IOptions<ScraperConfiguration>>().Value);
		}

		public IDictionary<object, object> Properties { get; init; } = new Dictionary<object, object>();

		public IConfigurationManager Configuration { get; init; } = new ConfigurationManager();

		public IHostEnvironment Environment { get; init; } = new HostingEnvironment();

		public ILoggingBuilder Logging { get; set; }

		public IMetricsBuilder Metrics { get; set; }


		public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder> configure = null) where TContainerBuilder : notnull
		{
			var builder = factory.CreateBuilder(Services);
			configure?.Invoke(builder);
		}

		public ScrapeApplication Build()
		{
			var provider = Services.BuildServiceProvider();
			return new ScrapeApplication(provider);
		}
	}
}
