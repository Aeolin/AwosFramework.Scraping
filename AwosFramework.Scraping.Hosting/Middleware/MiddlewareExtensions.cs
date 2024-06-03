using AwosFramework.Scraping.Binding;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Core.Results;
using AwosFramework.Scraping.Hosting.Builders;
using AwosFramework.Scraping.Middleware;
using AwosFramework.Scraping.Middleware.Http;
using AwosFramework.Scraping.Middleware.Result;
using AwosFramework.Scraping.Middleware.Routing;
using AwosFramework.Scraping.ResultHandling;
using AwosFramework.Scraping.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting.Middleware
{
	public static class MiddlewareExtensions
	{
		public static IScrapeApplicationBuilder UseInjectedMiddleware<T>(this IScrapeApplicationBuilder builder) where T : IMiddleware
		{
			builder.UseMiddleware(x => ActivatorUtilities.CreateInstance<T>(x));
			return builder;
		}

		public static IScrapeApplicationBuilder UseMiddleware<T>(this IScrapeApplicationBuilder builder) where T : IMiddleware, new()
		{
			builder.UseMiddleware(new T());
			return builder;
		}

		public static IScrapeApplicationBuilder UseMiddleware(this IScrapeApplicationBuilder builder, IMiddleware middleware)
		{
			builder.UseMiddleware(x => middleware);
			return builder;
		}

		public static IScrapeApplicationBuilder UseRouting(this IScrapeApplicationBuilder builder)
		{
			builder.UseInjectedMiddleware<RoutingMiddleware>();
			return builder;
		}

		public static IScrapeApplicationBuilder UseDefaultRoute(this IScrapeApplicationBuilder builder, IScrapeRequestHandler scrapeRequestHandler)
		{
			builder.UseMiddleware(x => new DefaultRouteMiddleware(scrapeRequestHandler));
			return builder;
		}

		public static IScrapeApplicationBuilder UseDefaultRoute(this IScrapeApplicationBuilder builder, Func<MiddlewareContext, Task<IScrapeResult>> func)
		{
			var handler = new AnonymousScrapeRequestHandler(func);
			builder.UseDefaultRoute(handler);
			return builder;
		}

		public static IScrapeApplicationBuilder MapControllers(this IScrapeApplicationBuilder builder)
		{
			var factory = builder.Services.GetRequiredService<RouteMapFactory>();
			var binderFactory = builder.Services.GetRequiredService<IBinderFactory>();

			var controllers = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(x => x.GetTypes())
					.Where(x => x.IsAssignableTo(typeof(ScrapeController)) && x.IsAbstract == false)
					.ToArray();

			var methods = controllers
					.SelectMany(x => x.GetMethods())
					.Where(x => x.GetCustomAttribute<RouteAttribute>() != null)
					.ToArray();

			foreach (var method in methods)
			{
				var controllerMethod = new ControllerMethod(method, binderFactory);
				factory.AddControllerMethod(controllerMethod);
			}

			var defaultRoute = controllers
					.SelectMany(x => x.GetMethods())
					.FirstOrDefault(x => x.GetCustomAttribute<DefaultRouteAttribute>() != null);

			if (defaultRoute != null)
			{
				var controllerMehod = new ControllerMethod(defaultRoute, binderFactory);
				builder.UseDefaultRoute(controllerMehod);
			}

			return builder;
		}

		public static IScrapeApplicationBuilder UseHttpRequests(this IScrapeApplicationBuilder builder, Action<HttpRequestMiddlewareOptions> configure = null)
		{
			var config = new HttpRequestMiddlewareOptions();
			configure?.Invoke(config);
			builder.UseMiddleware<HttpRequestMiddleware>();
			
			if (config.UseHtml)
				builder.UseMiddleware<HtmlResultMiddleware>();

			if (config.UseJson)
				builder.UseMiddleware<JsonResultMiddleware>();
			
			return builder;
		}

		public static IScrapeApplicationBuilder UseResultHandling(this IScrapeApplicationBuilder builder)
		{
			builder.UseInjectedMiddleware<ResultHandlingMiddleware>();
			return builder;
		}

		public static IScrapeApplicationBuilder AddResultHandler(this IScrapeApplicationBuilder builder, IResultHandler handler)
		{
			builder.UseResultHandler(x => handler);
			return builder;
		}

		public static IScrapeApplicationBuilder AddResultHandler<T>(this IScrapeApplicationBuilder builder) where T : IResultHandler, new()
		{
			builder.AddResultHandler(new T());
			return builder;
		}

		public static IScrapeApplicationBuilder AddInjectedResultHandler<T>(this IScrapeApplicationBuilder builder) where T : IResultHandler
		{
			builder.UseResultHandler(x => ActivatorUtilities.CreateInstance<T>(x));
			return builder;
		}

	}
}
