using AwosFramework.Scraping.Hosting;
using AwosFramework.Scraping.Hosting.Middleware;
using AwosFramework.Scraping.PuppeteerRequestor.CloudFlare;
using AwosFramework.Scraping.PuppeteerRequestor.CloudFlare.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddPuppeteer(this IServiceCollection collection, Action<BrowserPoolSettings> configure = null)
		{
			collection.AddOptions<BrowserPoolSettings>();
			if (configure != null)
				collection.Configure<BrowserPoolSettings>(configure);

			collection.AddSingleton<BrowserPool>();
			collection.AddSingleton<PuppeteerRequestMiddlware>();
			return collection;
		}

		public static IServiceCollection AddCloudFlareBypass(this IServiceCollection collection)
		{
			collection.AddSingleton<ICloudFlareDetector, CloudFlareDetector>();
			collection.AddSingleton<ICloudFlareSolver, CloudFlareSolver>();
			collection.AddSingleton<ICloudFlareClearanceProvider, CloudFlareClearanceProvider>();
			collection.AddTransient<CloudFlareHandler>();
			collection.AddHttpClient(Options.DefaultName).AddHttpMessageHandler<CloudFlareHandler>();
			return collection;
		}

		public static IScrapeApplicationBuilder UsePuppeteer(this IScrapeApplicationBuilder builder)
		{
			builder.UseMiddleware(services => services.GetRequiredService<PuppeteerRequestMiddlware>());
			return builder;
		}

	}
}
