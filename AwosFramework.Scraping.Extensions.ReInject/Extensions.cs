using AwosFramework.Scraping.Binding;
using AwosFramework.Scraping.Binding.DefaultBinders;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Routing;
using ReInject.Interfaces;
using System.Reflection;
using System.Runtime.InteropServices;
using RouteAttribute = AwosFramework.Scraping.Routing.RouteAttribute;

namespace AwosFramework.Scraping.Extensions.ReInject
{
	public static class Extensions
	{
		private static readonly IBinderGenerator[] DEFAULT_GENERATORS = [
			new BodyBinderGenerator(),
			new HtmlBinderGenerator(),
			new JobBinderGenerator(),
			new QueryBinderGenerator(),
			new RouteBinderGenerator()
		];

		public static IDependencyContainer AddDefaultBinders(this IDependencyContainer container, Action<BinderFactory> factoryAction = null)
		{
			var factory = new BinderFactory(DEFAULT_GENERATORS);
			factoryAction?.Invoke(factory);
			container.AddSingleton<IBinderFactory>(factory);
			return container;
		}

		public static IDependencyContainer MapScrapeControllers(this IDependencyContainer container)
		{
			var controllers = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(x => x.IsAssignableTo(typeof(ScrapeController)) && x.IsAbstract == false)
				.ToArray();

			var methods = controllers
				.SelectMany(x => x.GetMethods())
				.Where(x => x.GetCustomAttribute<RouteAttribute>() != null)
				.ToArray();

			var binderFactory = container.GetInstance<IBinderFactory>();
			if(binderFactory == null)
				throw new InvalidOperationException($"No binder factory found, please call {nameof(AddDefaultBinders)} first or register a {nameof(IBinderFactory)} instance yourself");
			
			var router = new RouteMap();
			foreach(var method in methods)
			{
				var route = new ControllerMethod(method, binderFactory);
				router.AddRoute(route);
			}

			container.AddSingleton(router);
			return container;
		}
	}
}
