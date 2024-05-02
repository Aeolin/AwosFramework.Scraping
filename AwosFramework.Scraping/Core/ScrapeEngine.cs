using AwosFramework.Scraping.Core.Results;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AwosFramework.Scraping.Core
{
	public class ScrapeEngine
	{
		private readonly HttpClient _client;
		private IServiceProvider _container;
		private ILogger _logger;
		private RouteMap _router;
		private static readonly string[] JSON_TYPES = ["application/json", "application/vnd.api+json"];
		public bool IsScraping { get; private set; } = false;

		public ScrapeEngine(IServiceProvider container, ILoggerFactory factory, RouteMap router)
		{
			_container = container;
			var client = container.GetService<HttpClient>();
			if (client == null)
				throw new InvalidOperationException($"Couldn't get an http client from service provider");

			_client = client;
			_logger = factory.CreateLogger<ScrapeEngine>();
			_router = router;
		}

		public async Task<IScrapeResult> ScrapeAsync(ScrapeJob job)
		{

			if (_router.TryRoute(job.Uri, out var method, out var match) == false)
				return new FailedResult($"No matching route for url {job.Uri} found");

			var type = method.ControllerType;
			var controller = _container.GetService(type) as ScrapeController;
			if (controller == null)
				return new FailedResult($"Couldn't resolve the controller of type {type.Name}");

			HtmlDocument html = null;
			JsonDocument json = null;
			BinaryContent binary = null;
			if (job.Request != null)
			{
				var response = await _client.SendAsync(job.Request);
				if (response == null)
					return new FailedResult($"Result of request to {job.Request.RequestUri} returned null");

				if (response.IsSuccessStatusCode == false)
					return new FailedResult($"Status code {response.StatusCode} - {(int)response.StatusCode} didn't indicate success\nJob: {job.Uri}");

				var mimeType = response?.Content?.Headers?.ContentType?.MediaType;
				if (mimeType == "text/html")
				{
					var rhtml = await response.Content.ReadAsStringAsync();
					html = new HtmlDocument();
					html.LoadHtml(rhtml);
				}
				else if (mimeType != null && JSON_TYPES.Contains(mimeType?.ToLower()))
				{
					var rjson = await response.Content.ReadAsStringAsync();
					json = JsonDocument.Parse(rjson);
				}
				else
				{
					binary = BinaryContent.OfStream(response.Content.ReadAsStream(), mimeType);
				}

				if (html == null && json == null)
					_logger.LogWarning($"couldn't extract any content, binding might fail");
			}

			var queryData = job.Uri.Query
				.TrimStart('?')
				.Split('&')
				.Select(x => x.Split('='))
				.Where(x => x != null && x.Length == 2 && string.IsNullOrEmpty(x[0]))
				.ToFrozenDictionary(x => x[0], x => x[1]);

			try
			{
				var context = new ScrapingContext { BinaryContent = binary, HtmlContent = html, JsonContent = json, RouteData = match.Data, QueryData = queryData, JobData = job.Data };
				var result = await method.CallAsync(controller, context);
				_logger.LogInformation("Executed job[{0}]({1}) for url {2}, success: {3}", job.Id, job.Data, job.Uri, !result?.Failed);
				return result;
			}
			catch (Exception ex)
			{
				return new FailedResult(ex, $"Error in handler method {method.Name}");
			}
		}

	}
}
