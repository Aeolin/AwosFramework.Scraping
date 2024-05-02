using AwosFramework.Scraping.Core.Results;
using AwosFramework.Scraping.Utils;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Core
{
	public class ScrapeJob : IScrapeJob
	{
		public int RetryCount { get; private set; }
		public Guid Id { get; init; }
		public Uri Uri { get; init; }
		public HttpRequestMessage Request { get; set; }
		public int Priority { get; init; }
		public bool AllowPartialResult { get; init; }


		public ScrapeJob Parent { get; init; }
		public IScrapeResult Result { get; set; }
		public object Data { get; set; }

		public T GetData<T>() => (T)Data;

		public void Retry()
		{
			RetryCount++;
			var request = Request.Clone();
		}

		public static ScrapeJob Post<T>(string url, T postData, int priortiy = 0, object data = null)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, url);
			request.Content = JsonContent.Create(data);
			return new ScrapeJob(url, priortiy, null, data, request);
		}

		public static ScrapeJob Get(string url)
		{
			return new ScrapeJob(url, 0, null, null, null);
		}

		public static ScrapeJob Get(string url, int priority = 0, object data = null, bool partialResult = false)
		{
			return new ScrapeJob(url, priority, null, data, null, partialResult);
		}

		public static ScrapeJob Get(HttpRequestMessage request, int priority = 0, object data = null, bool partialResult = false)
		{
			return new ScrapeJob(request.RequestUri, priority, null, data, request, partialResult);
		}

		public ScrapeJob(string uri, int priority = 0, ScrapeJob parent = null, object data = null, HttpRequestMessage request = null, bool partialResult = false) : this(new Uri(uri), priority, parent, data, request, partialResult)
		{
			
		}

		public ScrapeJob(Uri uri, int priority = 0, ScrapeJob parent = null, object data = null, HttpRequestMessage request = null, bool partialResult = false)
		{
			RetryCount = 0;
			Id = Guid.NewGuid();
			Uri = uri;
			Parent = parent;
			Data=data;
			Priority = priority;
			Request=request ?? (uri == null ? null : new HttpRequestMessage(HttpMethod.Get, uri));
			AllowPartialResult = partialResult;
		}
	}
}
