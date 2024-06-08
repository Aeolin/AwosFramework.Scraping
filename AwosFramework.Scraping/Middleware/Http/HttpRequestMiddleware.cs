using AwosFramework.Scraping.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware.Http
{
	public class HttpRequestMiddleware : IMiddleware
	{
		private readonly HttpRequestMiddlewareConfiguration _config;

		public HttpRequestMiddleware(HttpRequestMiddlewareConfiguration config)
		{
			_config=config;
		}

		public async Task<bool> ExecuteAsync(MiddlewareContext context)
		{
			if (context.ScrapeJob is not HttpJob job || context.RequestHandeled)
				return true;

			if(_config.Filter != null && !_config.Filter(context))
				return !_config.CancelMiddlewareOnFilterMismatch;

			var client = context.ServiceProvider.GetRequiredService<HttpClient>();
			var response = await client.SendAsync(job.Request);
			if (response.IsSuccessStatusCode)
			{
				context.AddComponent(response);
				var queryData = job.Uri.Query.TrimStart('?').Split('&').Select(x => x.Split('=')).Where(x => x != null && x.Length == 2 && string.IsNullOrEmpty(x[0])).ToFrozenDictionary(x => x[0], x => x[1]);
				context.AddComponent(new QueryData(queryData));

				if (response.Content != null)
				{
					var mimeType = response?.Content?.Headers?.ContentType?.MediaType?.ToLower();
					var stream = await response.Content.ReadAsStreamAsync();
					var result = new HttpResponseData(stream, mimeType);
					context.AddRequestResult(result);
				}
				return true;
			}
			else
			{
				return !_config.CancelMiddlewareOnHttpError;
			}
		}
	}
}
