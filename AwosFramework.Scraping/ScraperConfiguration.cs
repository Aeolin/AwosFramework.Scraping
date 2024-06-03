using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping
{
	public class ScraperConfiguration
	{
		public int MaxThreads { get; set; } = Environment.ProcessorCount;
		public int MaxRetries { get; set; } = 3;
		public string ScraperName { get; set; }
	}
}
