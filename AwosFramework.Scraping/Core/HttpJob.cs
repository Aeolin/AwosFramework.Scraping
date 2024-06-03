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
	public class HttpJob : IScrapeJob
	{
		public int RetryCount { get; private set; }
		public Guid Id { get; init; }
		public Uri Uri { get; init; }
		public HttpRequestMessage Request { get; private set; }
		public int Priority { get; init; }
		public bool AllowPartialResult { get; init; }


		public HttpJob Parent { get; init; }
		public IScrapeResult Result { get; set; }
		public object Data { get; set; }


		public T GetData<T>() => (T)Data;

		public bool Retry(int maxRetries)
		{
			RetryCount++;
			if (RetryCount > maxRetries)
				return false;

			Request = Request.ResendableCopy();
			return true;
		}

		public static HttpJob Post<T>(string url, T postData, int priortiy = 0, object data = null)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, url);
			request.Content = JsonContent.Create(data);
			return new HttpJob(url, priortiy, null, data, request);
		}

		public static HttpJob Get(string url)
		{
			return new HttpJob(url, 0, null, null, null);
		}

		public static HttpJob Get(Uri url)
		{
			return new HttpJob(url, 0, null, null, null);
		}

		public static HttpJob Get(string url, int priority = 0, object data = null, bool partialResult = false)
		{
			return new HttpJob(url, priority, null, data, null, partialResult);
		}

		public static HttpJob Get(Uri url, int priority = 0, object data = null, bool partialResult = false)
		{
			return new HttpJob(url, priority, null, data, null, partialResult);
		}

		public static HttpJob Get(HttpRequestMessage request, int priority = 0, object data = null, bool partialResult = false)
		{
			return new HttpJob(request.RequestUri, priority, null, data, request, partialResult);
		}

		public HttpJob(string uri, int priority = 0, HttpJob parent = null, object data = null, HttpRequestMessage request = null, bool partialResult = false) : this(new Uri(uri), priority, parent, data, request, partialResult)
		{
			
		}

		public HttpJob(Uri uri, int priority = 0, HttpJob parent = null, object data = null, HttpRequestMessage request = null, bool partialResult = false)
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
