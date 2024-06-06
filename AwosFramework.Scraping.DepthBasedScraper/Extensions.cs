using AwosFramework.Scraping.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.DepthBasedScraper
{
	public static class Extensions
	{
		public static bool ContainsAndAdd<T>(this ConcurrentBag<T> bag, T item)
		{
			if (bag.Contains(item))
				return true;

			bag.Add(item);
			return false;
		}


		public static IEnumerable<TResult> SelectWhere<TItem, TResult>(this IEnumerable<TItem> items, Func<TItem, (bool keep, TResult transformed)> func)
		{
			foreach (var item in items)
			{
				var res = func(item);
				if (res.keep)
					yield return res.transformed;
			}
		}


		public static async Task<IEnumerable<IScrapeJob>> GetInitialJobsAsync(this IServiceProvider services)
		{
			var jobs = new List<HttpJob>();
			var cfg = services.GetRequiredService<DepthBasedScrapingConfig>();
			if (cfg.StartUrls != null)
				jobs.AddRange(cfg.StartUrls.Select(HttpJob.Get));

			string[] urls = Array.Empty<string>();
			if (cfg.StartUrlsFile?.EndsWith(".txt") ?? false)
			{
				urls = await File.ReadAllLinesAsync(cfg.StartUrlsFile);
				jobs.AddRange(urls.Select(HttpJob.Get));
			}
			else if (cfg.StartUrlsFile?.EndsWith(".json") ?? false)
			{
				var json = await File.ReadAllTextAsync(cfg.StartUrlsFile);
				urls = JsonSerializer.Deserialize<string[]>(json);
			}

			var opts = new UriCreationOptions();
			jobs.AddRange(urls.SelectWhere(x => (Uri.TryCreate(x, opts, out var uri), uri)).Select(x => HttpJob.Get(x, 10, new DepthData(1, x))));

			return jobs;
		}
	}
}
