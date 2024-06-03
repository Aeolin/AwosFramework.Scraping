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

		public static async Task<IEnumerable<IScrapeJob>> GetInitialJobsAsync(this IServiceProvider services)
		{
			var jobs = new List<HttpJob>();
			var cfg = services.GetRequiredService<DepthBasedScrapingConfig>();
			if (cfg.StartUrls != null)
				jobs.AddRange(cfg.StartUrls.Select(HttpJob.Get));

			if (cfg.StartUrlsFile?.EndsWith(".txt") ?? false)
			{
				var urls = await File.ReadAllLinesAsync(cfg.StartUrlsFile);
				jobs.AddRange(urls.Select(HttpJob.Get));
			}

			if (cfg.StartUrlsFile?.EndsWith(".json") ?? false)
			{
				var json = await File.ReadAllTextAsync(cfg.StartUrlsFile);
				var urls = JsonSerializer.Deserialize<string[]>(json);
				jobs.AddRange(urls.Select(x => HttpJob.Get(x, priority: 10)));
			}

			return jobs;
		}
	}
}
