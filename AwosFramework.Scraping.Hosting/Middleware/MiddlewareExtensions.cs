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
		public static IScrapeApplicationBuilder UseMiddleware<T>(this IScrapeApplicationBuilder builder, Func<IServiceProvider, T> factory) where T : IMiddleware
		{
			builder.Middleware.AddMiddleware(factory);
			return builder;
		}

		public static IScrapeApplicationBuilder UseMiddleware(this IScrapeApplicationBuilder builder, IMiddleware middleware)
		{
			builder.UseMiddleware(x => middleware);
			return builder;
		}

		public static IScrapeApplicationBuilder UseMiddleware<T>(this IScrapeApplicationBuilder builder) where T : IMiddleware, new()
		{
			builder.UseMiddleware(x => new T());
			return builder;
		}

		public static IScrapeApplicationBuilder UseUniqueMiddleware<T>(this IScrapeApplicationBuilder builder, Func<IServiceProvider, T> factory) where T : IMiddleware
		{
			builder.Middleware.AddUniqueMiddleware(factory);
			return builder;
		}

		public static IScrapeApplicationBuilder UseUniqueMiddleware<T>(this IScrapeApplicationBuilder builder) where T : IMiddleware, new()
		{
			builder.UseUniqueMiddleware(x => new T());
			return builder;
		}

		public static IScrapeApplicationBuilder UseUniqueMiddleware<T>(this IScrapeApplicationBuilder builder, T instance) where T : IMiddleware
		{
			builder.UseUniqueMiddleware(x => instance);
			return builder;
		}

		public static IScrapeApplicationBuilder UseInjectedMiddleware<T>(this IScrapeApplicationBuilder builder) where T : IMiddleware
		{
			builder.UseMiddleware(x => ActivatorUtilities.CreateInstance<T>(x));
			return builder;
		}

		public static IScrapeApplicationBuilder UseUniqueInjectedMiddleware<T>(this IScrapeApplicationBuilder builder) where T : IMiddleware
		{
			builder.Middleware.AddUniqueMiddleware(x => ActivatorUtilities.CreateInstance<T>(x));
			return builder;
		}

		public static IScrapeApplicationBuilder UseControllers(this IScrapeApplicationBuilder builder)
		{
			builder.UseUniqueMiddleware<ScrapeDataHandlerMiddleware>();
			return builder;
		}

		public static IScrapeApplicationBuilder UseRouting(this IScrapeApplicationBuilder builder)
		{
			builder.UseInjectedMiddleware<RoutingMiddleware>();
			return builder;
		}

		public static IScrapeApplicationBuilder UseDefaultRoute(this IScrapeApplicationBuilder builder, IScrapeDataHandler scrapeRequestHandler)
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

		public static IScrapeApplicationBuilder UseHttpRequests(this IScrapeApplicationBuilder builder)
		{
			builder.UseMiddleware(x => x.GetRequiredService<HttpRequestMiddleware>());
			return builder;
		}

		public static IScrapeApplicationBuilder UseDefaultContent(this IScrapeApplicationBuilder builder)
		{
			builder.UseHtmlContent();
			builder.UseJsonContent();
			return builder;
		}

		public static IScrapeApplicationBuilder UseHtmlContent(this IScrapeApplicationBuilder builder)
		{
			builder.UseUniqueMiddleware<HtmlResultMiddleware>();
			return builder;
		}

		public static IScrapeApplicationBuilder UseJsonContent(this IScrapeApplicationBuilder builder)
		{
			builder.UseUniqueMiddleware<JsonResultMiddleware>();
			return builder;
		}

		public static IScrapeApplicationBuilder UseResultHandling(this IScrapeApplicationBuilder builder)
		{
			builder.UseUniqueInjectedMiddleware<ResultHandlingMiddleware>();
			return builder;
		}

		public static IScrapeApplicationBuilder UseResultHandler(this IScrapeApplicationBuilder builder, Func<IServiceProvider, IResultHandler> factory)
		{
			builder.ResultHandlers.AddResultHandler(factory);
			return builder;
		}

		public static IScrapeApplicationBuilder UseResultHandler(this IScrapeApplicationBuilder builder, IResultHandler handler)
		{
			builder.UseResultHandling();
			builder.UseResultHandler(x => handler);
			return builder;
		}

		public static IScrapeApplicationBuilder UseResultHandler<T>(this IScrapeApplicationBuilder builder) where T : IResultHandler, new()
		{
			builder.UseResultHandling();
			builder.UseResultHandler(new T());
			return builder;
		}

		public static IScrapeApplicationBuilder UseInjectedResultHandler<T>(this IScrapeApplicationBuilder builder) where T : IResultHandler
		{
			builder.UseResultHandling();
			builder.UseResultHandler(x => ActivatorUtilities.CreateInstance<T>(x));
			return builder;
		}

	}
}
