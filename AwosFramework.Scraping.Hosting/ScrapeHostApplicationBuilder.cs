﻿using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Hosting.Builders;
using AwosFramework.Scraping.Middleware;
using AwosFramework.Scraping.Middleware.Result;
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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting
{
	public class ScrapeHostApplicationBuilder : IHostApplicationBuilder
	{
		public IServiceCollection Services { get; init; } = new ServiceCollection();

		public ScrapeHostApplicationBuilder()
		{
		}

		internal void AddDefaultServices()
		{
			Services.AddOptions();
			Services.AddLogging(x => Logging = x);
			Services.AddMetrics(x => Metrics = x);
			Services.AddOptions<ScraperConfiguration>();
			Services.Configure<ScraperConfiguration>(Configuration.GetSection("Scraper"));
			Services.AddTransient(x => x.GetRequiredService<IOptions<ScraperConfiguration>>().Value);
			Services.AddSingleton<MiddlewareCollectionFactory>();
			Services.AddSingleton<ResultHandlerCollectionFactory>();
			Services.AddSingleton<RouteMapFactory>();
			Services.AddScoped<MiddlewareCollection>(x => x.GetRequiredService<MiddlewareCollectionFactory>().Create(x));
			Services.AddScoped<ResultHandlerCollection>(x => x.GetRequiredService<ResultHandlerCollectionFactory>().Create(x));
			Services.AddSingleton(x => x.GetRequiredService<RouteMapFactory>().Create(x));
			Services.AddTransient<Scraper>();
			Services.AddScoped<ScrapeEngine>();
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
			return ActivatorUtilities.CreateInstance<ScrapeApplication>(provider, provider);
		}
	}
}
