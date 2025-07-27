using AwosFramework.Scraping.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.ResultHandling.Json
{
	public record BatchData<T>(string Category, int BatchNumber, int DataCount, T Data);

	public class JsonResultCategory<T>
	{
		public string CategoryName { get; init; }
		public string DirectoryName { get; init; }
		public NameTemplate<BatchData<T>> FileNameTemplate { get; init; }
		
		public Predicate<T> Filter { get; init; }
		private ConcurrentBag<T> _bag;
		public int BatchCount { get; private set; }
		public int BatchSize { get; init; }
		public JsonSerializerOptions SerializerOptions { get; init; }

		public JsonResultCategory(string categoryName, Predicate<T> filter, string directoryName, JsonSerializerOptions serializerOptions, int batchSize, string fileNameTemplate = null)
		{
			this.CategoryName = categoryName;
			this._bag = new ConcurrentBag<T>();
			this.Filter = filter;
			this.DirectoryName = directoryName;
			Directory.CreateDirectory(directoryName);
			SerializerOptions=serializerOptions;
			BatchSize=batchSize;
			FileNameTemplate = TemplateBuilder.BuildTemplate<BatchData<T>>(fileNameTemplate ?? $"{{{nameof(BatchData<T>.Category)}}}_batch_{{{nameof(BatchData<T>.BatchNumber)}:0000}}.json");
		}

		public bool Matches(T item) => Filter(item);

		public async Task SaveAsync(bool respectBatchSize = false)
		{
			if ((respectBatchSize == false || _bag.Count >= BatchSize) && _bag.Count > 0)
			{
				var data = Interlocked.Exchange(ref _bag, new ConcurrentBag<T>());
				var batchData = new BatchData<T>(CategoryName, BatchCount++, data.Count, data.First());
				var fileName = FileNameTemplate(batchData);
				using var file = File.Create(Path.Combine(DirectoryName, fileName));
				await JsonSerializer.SerializeAsync(file, data, SerializerOptions);
			}
		}

		public async Task HandleAsync(T data)
		{	
			_bag.Add(data);
			if (_bag.Count >= BatchSize)
				await SaveAsync(true);
		}
	}
}
