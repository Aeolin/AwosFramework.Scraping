using AwosFramework.Scraping.Binding;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Hosting.Builders;
using AwosFramework.Scraping.Hosting.ResultHandlers;
using AwosFramework.Scraping.ResultHandling.Json;
using AwosFramework.Scraping.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AwosFramework.Scraping.Hosting
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddBinderFactory(this IServiceCollection services, Action<BinderFactoryBuilder> configure)
		{
			var builder = new BinderFactoryBuilder();
			configure(builder);
			services.AddSingleton(builder.Build());
			return services;
		}

		public static IServiceCollection AddScraper(this IServiceCollection services, Action<IScraperFactoryBuilder> configure)
		{
			services.AddTransient<ScrapeEngine>();
			var factory = new ScraperFactory();
			configure?.Invoke(factory);
			services.AddSingleton(factory);
			services.AddTransient<Scraper>(provider =>
			{
				var sFactory = provider.GetRequiredService<ScraperFactory>();
				return sFactory.Build(provider);
			});

			return services;
		}

		public static IServiceCollection AddScrapeControllers(this IServiceCollection services)
		{
			services.AddSingleton(provider =>
			{
				var controllers = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(x => x.IsAssignableTo(typeof(ScrapeController)) && x.IsAbstract == false)
				.ToArray();

				var methods = controllers
					.SelectMany(x => x.GetMethods())
					.Where(x => x.GetCustomAttribute<RouteAttribute>() != null)
					.ToArray();

				var defaultRoute = controllers
					.SelectMany(x => x.GetMethods())
					.FirstOrDefault(x => x.GetCustomAttribute<DefaultRouteAttribute>() != null);

				var binderFactory = provider.GetRequiredService<IBinderFactory>();
				var router = new RouteMap();
				foreach (var method in methods)
				{
					var route = new ControllerMethod(method, binderFactory);
					router.AddRoute(route);
				}
				if (defaultRoute != null)
				{
					var defaultMethod = new ControllerMethod(defaultRoute, binderFactory);
					router.SetDefaultRoute(defaultMethod);
				}

				return router;
			});
			return services;
		}
	}
}
