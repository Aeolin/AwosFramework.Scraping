using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.DepthBasedScraper
{
	public class DepthBasedScrapingConfig
	{
		public int? MaxDepth { get; set; } = 1;
		public int? MaxPages { get; set; } = 100;
		public string StartUrlsFile { get; set; }
		public string OutputDirectory { get; set; }
		public string[] StartUrls { get; set; }

	}
}
