using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware.Http
{
	public class HtmlResultMiddleware : IMiddleware
	{
		public Task<bool> ExecuteAsync(MiddlewareContext context)
		{
			if (context.TryGetComponent<HttpResponseData>(out var response) && response.MimeType == "text/html")
			{
				var document = new HtmlDocument();
				document.Load(response.Stream);
				context.AddComponent(document);
			}

			return Task.FromResult(true);
		}
	}
}
