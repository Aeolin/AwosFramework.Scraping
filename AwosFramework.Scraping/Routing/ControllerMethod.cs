using AwosFramework.Scraping.Binding;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Core.Results;
using AwosFramework.Scraping.Middleware;
using AwosFramework.Scraping.Middleware.Http;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Routing
{
	public class ControllerMethod : IScrapeRequestHandler
	{
		private readonly RouteMatcher _matcher;
		private readonly IBinder[] _binders;
		private readonly MethodInfo _method;
		private readonly bool _isTask;

		public Type ControllerType { get; init; }
		public string Name { get; init; }

		public ControllerMethod(MethodInfo methodInfo, IBinderFactory binderFactory)
		{
			var attr = methodInfo.GetCustomAttribute<RouteAttribute>();
			if (attr != null)
				_matcher = new RouteMatcher(attr.Host, attr.Path);
			else if(methodInfo.GetCustomAttribute<DefaultRouteAttribute>() == null)
				throw new ArgumentException($"Method must either have a {nameof(RouteAttribute)} or {nameof(DefaultRouteAttribute)}", nameof(methodInfo));
			
			_binders = methodInfo.GetParameters().Select(p => binderFactory.CreateBinder(p, _matcher, p.HasDefaultValue ? p.DefaultValue : null)).ToArray();
			_method = methodInfo;
			_isTask = methodInfo.ReturnType.IsAssignableTo(typeof(Task<IScrapeResult>));
			ControllerType = methodInfo.DeclaringType;
			Name = methodInfo.Name;
		}

		public RouteMatchResult MatchResult(string route)
		{
			return _matcher?.Match(route) ?? RouteMatchResult.Failed;
		}

		public RouteMatchResult MatchResult(Uri route)
		{
			return _matcher?.Match(route) ?? RouteMatchResult.Failed;
		}

		public async Task<IScrapeResult> HandleAsync(MiddlewareContext context)
		{
			var controller = _method.IsStatic ? null : ActivatorUtilities.GetServiceOrCreateInstance(context.ServiceProvider, ControllerType);
			var objects = _binders.Select(b => b.Bind(context)).ToArray();
			if (controller is ScrapeController scrapeController)
				scrapeController.Setup(context.GetComponent<HtmlDocument>(), context.GetComponent<JsonDocument>(), context.GetComponent<HttpResponseData>(), context.ScrapeJob.Uri);

			var result = _method.Invoke(controller, objects);
			if (_isTask)
				return await(Task<IScrapeResult>)result;
			else
				return (IScrapeResult)result;
		}
	}
}
