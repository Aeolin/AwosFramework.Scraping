using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor
{
	public class BrowserPoolSettings
	{
		public LaunchOptions LaunchOptions { get; init; } = new LaunchOptions();
		public BrowserTag BrowserTag { get; init; } = BrowserTag.Latest;
		public bool DownloadAutomatically { get; init; } = false;
		public int Count { get; init; } = Environment.ProcessorCount / 2;
	}
}
