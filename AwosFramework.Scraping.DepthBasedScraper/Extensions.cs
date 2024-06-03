using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	}
}
