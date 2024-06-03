using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware.Http
{

	public class JsonResultMiddleware : IMiddleware
	{
		private static readonly string[] JSON_TYPES = ["application/json", "application/vnd.api+json"];

		public async Task<bool> ExecuteAsync(MiddlewareContext context)
		{
			if(context.TryGetComponent<HttpResponseData>(out var response) && JSON_TYPES.Contains(response.MimeType))
			{
				var json = await JsonDocument.ParseAsync(response.Stream);
				context.AddComponent(json);
			}

			return true;
		}
	}
}
