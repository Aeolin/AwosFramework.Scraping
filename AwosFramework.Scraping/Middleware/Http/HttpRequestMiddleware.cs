using AwosFramework.Scraping.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AwosFramework.Scraping.Middleware.Http
{
	public class HttpRequestMiddleware : IMiddleware
	{
		private readonly HttpRequestMiddlewareConfiguration _config;
		private readonly ILogger _logger;

		public HttpRequestMiddleware(HttpRequestMiddlewareConfiguration config, ILoggerFactory loggerFactory)
		{
			_config=config;
			_logger = loggerFactory.CreateLogger<HttpRequestMiddleware>();
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
				var queryData = HttpUtility.UrlDecode(job.Uri.Query).TrimStart('?').Split('&').Select(x => x.Split('=')).Where(x => x != null && x.Length == 2 && string.IsNullOrEmpty(x[0]) == false).ToFrozenDictionary(x => x[0], x => x[1]);
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
				if(_config.WaitOnRateLimit && response.StatusCode == HttpStatusCode.TooManyRequests && response.Headers.TryGetValues("x-ratelimit-reset", out var timeouts) && int.TryParse(timeouts.First(), out var timeoutSeconds))
				{
					timeoutSeconds = Math.Min(timeoutSeconds, _config.MaxRateLimitWaitSeconds);
					_logger.LogError("Hit ratelimit, waiting for {Timeout}s", timeoutSeconds);
					await Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));
				}
				else
				{
					_logger.LogError("Error during HTTP Request to {Url} status code: {StatusCode}, reason: {ReasonPhrase}", job.Request.RequestUri, response.StatusCode, response.ReasonPhrase);
				}

				return !_config.CancelMiddlewareOnHttpError;
			}
		}
	}
}
