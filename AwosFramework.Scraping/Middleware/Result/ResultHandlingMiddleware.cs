using AwosFramework.Scraping.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware.Result
{
	public class ResultHandlingMiddleware : IMiddleware
	{
		private readonly ResultHandlerCollection _resultHandlers;

		public ResultHandlingMiddleware(ResultHandlerCollection resultHandlers)
		{
			_resultHandlers=resultHandlers;
		}

		public async Task<bool> ExecuteAsync(MiddlewareContext context)
		{
			var result = context.GetRequiredComponent<IScrapeResult>();
			foreach(var handler in _resultHandlers)
			{
				foreach(var obj in result.Data)
				{
					if (handler.CanHandle(obj))
						await handler.HandleAsync(obj);
				}
			}

			return true;
		}
	}
}
