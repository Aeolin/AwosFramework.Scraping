using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.DepthBasedScraper
{
	public class ScrapedPage
	{
		public string Url { get; set; }
		public int Depth { get; set; }
		public string Html { get; set; }
	}
}
