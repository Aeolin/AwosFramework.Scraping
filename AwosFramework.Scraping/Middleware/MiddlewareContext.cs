using AwosFramework.Scraping.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware
{
	public class MiddlewareContext : IDisposable
	{
		public IScrapeJob ScrapeJob { get; init; }
		public IServiceProvider ServiceProvider { get; init; }
		public ILogger Logger { get; init; }
		private readonly Dictionary<(Type, object), object> _components = new Dictionary<(Type, object), object>();


		public MiddlewareContext(IScrapeJob scrapeJob, IServiceProvider provider, ILogger logger)
		{
			ScrapeJob=scrapeJob;
			ServiceProvider=provider;
			Logger = logger;
		}

		public void AddComponent<T>(T obj, string key = null)
		{
			_components.Add((typeof(T), key), obj);
		}

		public T GetRequiredComponent<T>(string key = null)
		{
			if (TryGetComponent<T>(out var result, key))
				return result;
			else
				throw new InvalidOperationException($"Component with key {key} not found in middleware components");
		}

		public T GetComponent<T>(string key = null)
		{
			if (_components.TryGetValue((typeof(T), key), out var obj) && obj is T tRes)
				return tRes;
			return default;
		}

		public bool TryGetComponent<T>(out T result, string key = null)
		{
			if (_components.TryGetValue((typeof(T), key), out var obj) && obj is T tRes)
			{
				result = tRes;
				return true;
			}
			else
			{
				result = default;
				return false;
			}
		}

		public void Dispose()
		{
			foreach (var disposable in _components.Values.OfType<IDisposable>())
				disposable.Dispose();
		}
	}
}
