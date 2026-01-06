using AwosFramework.Scraping.Core.Results;
using AwosFramework.Scraping.Middleware.Http;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Core
{
	public abstract class ScrapeController
	{
		protected HtmlDocument Content { get; private set; }
		protected JsonDocument JsonContent { get; private set; }
		protected Uri Url { get; private set; }
		public HttpResponseData BinaryContent { get; private set; }
		private static readonly ScrapeResult _emptyResult = new ScrapeResult(null, null);

		public void Setup(HtmlDocument content, JsonDocument json, HttpResponseData binaryContent, Uri url)
		{
			Content = content;
			JsonContent = json;
			BinaryContent = binaryContent;
			Url = url;
		}

		public IScrapeResult Follow(IEnumerable<IScrapeJob> jobs) => new FollowResult(jobs);
		public IScrapeResult Follow(params IScrapeJob[] jobs) => new FollowResult(jobs);

		public IScrapeResult OkFollow(object[] data, IEnumerable<IScrapeJob> jobs) => new ScrapeResult(jobs, data);
		public IScrapeResult OkFollow(IEnumerable<IScrapeJob> jobs, params object[] data) => new ScrapeResult(jobs, data);
		public IScrapeResult OkFollow<T>(IEnumerable<IScrapeJob> jobs, IEnumerable<T> data) => new ScrapeResult(jobs, data.Cast<object>().ToArray());
		public IScrapeResult OkFollow<T>(IScrapeJob job, IEnumerable<T> data) => new ScrapeResult([job], data.Cast<object>().ToArray());
		public IScrapeResult OkFollow(IScrapeJob job, params object[] data) => new ScrapeResult([job], data);
		public IScrapeResult OkFollow(object[] data, params HttpJob[] jobs) => new ScrapeResult(jobs, data);

		public IScrapeResult Ok(params object[] data) => new ScrapeResult(null, data);
		public IScrapeResult Empty() => _emptyResult;
		public IScrapeResult Fail(Exception exception, string errorMessage) => new FailedResult(exception, errorMessage);
		public IScrapeResult Fail(string errorMessage) => new FailedResult(errorMessage);
	}
}
