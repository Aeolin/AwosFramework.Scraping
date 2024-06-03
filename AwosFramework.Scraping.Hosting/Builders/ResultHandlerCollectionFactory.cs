using AwosFramework.Scraping.Middleware.Result;
using AwosFramework.Scraping.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting.Builders
{
	public class ResultHandlerCollectionFactory
	{
		private readonly List<Func<IServiceProvider, IResultHandler>> _resultHandlers = new List<Func<IServiceProvider, IResultHandler>>();

		public void AddResultHandler(Func<IServiceProvider, IResultHandler> resultHandler)
		{
			_resultHandlers.Add(resultHandler);
		}

		public ResultHandlerCollection Create(IServiceProvider provider)
		{
			var resultHandlers = _resultHandlers.Select(m => m(provider)).ToList();
			return new ResultHandlerCollection(resultHandlers);
		}
	}
}
