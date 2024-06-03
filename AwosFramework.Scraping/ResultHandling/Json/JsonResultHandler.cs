using AwosFramework.Scraping.ResultHandling;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.ResultHandling.Json
{
	public class JsonResultHandler<T> : IResultHandler
	{
		public string Directory { get; init; }
		public int BatchSize { get; init; }
		public int BatchCount { get; private set; }
		public string FileName { get; private set; }
		public JsonSerializerOptions SerializerOptions { get; init; }

		private ConcurrentBag<T> _bag = new ConcurrentBag<T>();
		private Func<T, bool> _filter;

		public JsonResultHandler(string directory, int batchSize, string fileName = null, Func<T, bool> filter = null, JsonSerializerOptions serializerOptions = null)
		{
			Directory = directory;
			System.IO.Directory.CreateDirectory(Directory);
			BatchSize = batchSize;
			FileName = fileName ?? $"{typeof(T).Name.ToLower()}_batch_{{0}}.json";
			SerializerOptions = serializerOptions ?? JsonSerializerOptions.Default;
			_filter = filter;
		}

		[HandleResult(ResultMatchType.Exact)]
		public void Handle(T data)
		{
			if (_filter != null && _filter(data) == false)
				return;

			_bag.Add(data);
			if (_bag.Count >= BatchSize)
			{
				Save();
			}
		}

		public void Save()
		{
			if (_bag.Count > 0)
			{
				var data = Interlocked.Exchange(ref _bag, new ConcurrentBag<T>());
				using var file = File.Create(Path.Combine(Directory, string.Format(FileName, BatchCount++, BatchSize)));
				JsonSerializer.Serialize(file, data, SerializerOptions);
			}
		}

		public void Dispose()
		{
			Save();
		}

		public Task HandleAsync(object obj)
		{
			Handle((T)obj);
			return Task.CompletedTask;
		}

		public bool CanHandle(object obj)
		{
			return obj is T tObj && (_filter?.Invoke(tObj) ?? true);
		}
	}
}
