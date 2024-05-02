using AwosFramework.Scraping.Core.Results;
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
		public BinaryContent BinaryContent { get; private set; }
		public void Setup(HtmlDocument content, JsonDocument json, BinaryContent binaryContent)
		{
			Content = content;
			JsonContent = json;
			BinaryContent = binaryContent;
		}

		public IScrapeResult Follow(IEnumerable<ScrapeJob> jobs) => new FollowResult(jobs);
		public IScrapeResult Follow(params ScrapeJob[] jobs) => new FollowResult(jobs);

		public IScrapeResult OkFollow(object[] data, IEnumerable<ScrapeJob> jobs) => new ScrapeResult(jobs, data);
		public IScrapeResult OkFollow(IEnumerable<ScrapeJob> jobs, params object[] data) => new ScrapeResult(jobs, data);
		public IScrapeResult OkFollow<T>(IEnumerable<ScrapeJob> jobs, IEnumerable<T> data) => new ScrapeResult(jobs, data.Cast<object>().ToArray());
		public IScrapeResult OkFollow(ScrapeJob job, params object[] data) => new ScrapeResult([job], data);
		public IScrapeResult OkFollow(object[] data, params ScrapeJob[] jobs) => new ScrapeResult(jobs, data);

		public IScrapeResult Ok(params object[] data) => new ScrapeResult(null, data);

		public IScrapeResult Fail(Exception exception) => new FailedResult(exception);
	}
}
