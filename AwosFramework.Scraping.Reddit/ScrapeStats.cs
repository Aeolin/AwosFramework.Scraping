using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Reddit
{
	public class ScrapeStats
	{
		public ScrapeStats(RedditScraperConfig config)
		{
			SubCounts = config.Subreddits.ToFrozenDictionary(x => x, _ => new AtomicInteger(0));
		}

		public  FrozenDictionary<string, AtomicInteger> SubCounts { get; init; }
		public AtomicInteger PostCount { get; init; } =  new();
	}
}
