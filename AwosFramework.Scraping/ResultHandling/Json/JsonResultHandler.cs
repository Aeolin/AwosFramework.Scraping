using AwosFramework.Scraping.ResultHandling;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
		private SemaphoreSlim _saveSemaphore;

		public JsonResultHandler(string directory, int batchSize, string fileName = null, Func<T, bool> filter = null, JsonSerializerOptions serializerOptions = null)
		{
			Directory = directory;
			System.IO.Directory.CreateDirectory(Directory);
			BatchSize = batchSize;
			FileName = fileName ?? $"{typeof(T).Name.ToLower()}_batch_{{0}}.json";
			SerializerOptions = serializerOptions ?? JsonSerializerOptions.Default;
			_filter = filter;
			_saveSemaphore = new SemaphoreSlim(1);
		}

		public async Task SaveAsync(bool respectBatchSize = false)
		{
			if ((respectBatchSize == false || _bag.Count >= BatchSize) && _bag.Count > 0)
			{
				var data = Interlocked.Exchange(ref _bag, new ConcurrentBag<T>());
				using var file = File.Create(Path.Combine(Directory, string.Format(FileName, BatchCount++, BatchSize)));
				await JsonSerializer.SerializeAsync(file, data, SerializerOptions);
			}
		}

		public async Task HandleAsync(object data)
		{
			if (data is not T tData || (_filter != null && _filter(tData) == false))
				return;

			_bag.Add(tData);
			if (_bag.Count >= BatchSize)
				await SaveAsync(true);
		}

		public Task SaveAsync() => SaveAsync(false);

		public void Dispose() => SaveAsync().RunSynchronously();
	}
}
