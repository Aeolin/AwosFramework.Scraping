using AwosFramework.Scraping.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting.Builders
{
	public class MiddlewareCollectionFactory
	{
		private readonly List<Func<IServiceProvider, IMiddleware>> _middleware = new List<Func<IServiceProvider, IMiddleware>>();
		private readonly HashSet<Type> _middlewareTypes = new HashSet<Type>();

		public void AddMiddleware<T>(Func<IServiceProvider, T> middleware) where T : IMiddleware
		{
			_middleware.Add(x => middleware(x));
			_middlewareTypes.Add(typeof(T));
		}

		public void AddUniqueMiddleware<T>(Func<IServiceProvider, T> middleware) where T : IMiddleware){
			if (HasMiddleware<T>())
				return;

			AddMiddleware(middleware);
		}

		public bool HasMiddleware<T>() => _middlewareTypes.Contains(typeof(T));

		public MiddlewareCollection Create(IServiceProvider provider)
		{
			var middleware = _middleware.Select(m => m(provider)).ToList();
			return new MiddlewareCollection(middleware);
		}
	}
}
