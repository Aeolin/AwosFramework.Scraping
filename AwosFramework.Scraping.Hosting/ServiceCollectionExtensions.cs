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
	}
}
