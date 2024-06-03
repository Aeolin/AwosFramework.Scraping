using AwosFramework.Scraping.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting.Builders
{
	public class MiddlewareCollectionFactory
	{
		private readonly List<Func<IServiceProvider, IMiddleware>> _middleware = new List<Func<IServiceProvider, IMiddleware>>();

		public void AddMiddleware(Func<IServiceProvider, IMiddleware> middleware)
		{
			_middleware.Add(middleware);
		}

		public MiddlewareCollection Create(IServiceProvider provider)
		{
			var middleware = _middleware.Select(m => m(provider)).ToList();
			return new MiddlewareCollection(middleware);
		}
	}
}
