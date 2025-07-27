using AwosFramework.Scraping.ResultHandling;
using AwosFramework.Scraping.Utils;
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
		public JsonResultCategory<T>[] Categories { get; init; }
		public JsonSerializerOptions SerializerOptions { get; init; }

		private Predicate<T>? _filter;
		private readonly SemaphoreSlim _saveSemaphore = new(1);

		public JsonResultHandler(IEnumerable<JsonResultCategory<T>> categories, Predicate<T>? filter = null) 
		{
			this.Categories = categories.ToArray();
			this._filter = filter;
		}
		
		public async Task SaveAsync(bool respectBatchSize = false)
		{
			if (_saveSemaphore.CurrentCount == 0)
				return; // ignore save calls while already saving

			await _saveSemaphore.WaitAsync();
			await Task.WhenAll(Categories.Select(x => x.SaveAsync(respectBatchSize)));
			_saveSemaphore.Release();
		}

		public async Task HandleAsync(object data)
		{
			if (data is not T tData || (_filter != null && _filter(tData) == false))
				return;

			var category = Categories.FirstOrDefault(x => x.Matches(tData));
			if(category != null)
				await category.HandleAsync(tData);	
		}

		public Task SaveAsync() => SaveAsync(false);

		public void Dispose() => SaveAsync().RunSynchronously();
	}
}
