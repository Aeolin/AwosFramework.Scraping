using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor
{
	public class BrowserPool : IDisposable, IBrowserPool
	{
		private readonly Queue<IBrowser> _browsers;
		private readonly Task _initTask;
		private readonly SemaphoreSlim _browserSemaphore;
		private readonly BrowserPoolSettings _config;

		public BrowserPool(ILoggerFactory factory, IOptions<BrowserPoolSettings> settings)
		{
			_browsers = new Queue<IBrowser>();
			_config = settings.Value;
			_initTask = InitializeAsync(factory);
			_browserSemaphore = new SemaphoreSlim(_config.Count);
		}

		public async Task<IBrowser> GetBrowserAsync()
		{
			await _initTask;
			await _browserSemaphore.WaitAsync();

			return _browsers.Dequeue();
		}

		public async Task ReturnBrowserAsync(IBrowser browser)
		{
			var pages = await browser.PagesAsync();
			await Task.WhenAll(pages.Select(p => p.CloseAsync()));
			_browsers.Enqueue(browser);
			_browserSemaphore.Release();
		}

		protected async Task InitializeAsync(ILoggerFactory factory)
		{
			if (_config.DownloadAutomatically)
			{
				var options = new BrowserFetcherOptions { Browser = _config.LaunchOptions.Browser, Path = _config.LaunchOptions.ExecutablePath };
				var fetcher = new BrowserFetcher(options);
				await fetcher.DownloadAsync(_config.BrowserTag);
			}

			for (int i = 0; i < _config.Count; i++)
			{
				var browserInstance = await Puppeteer.LaunchAsync(_config.LaunchOptions);
				_browsers.Enqueue(browserInstance);
			}
		}

		public void Dispose()
		{
			foreach (var browser in _browsers)
				browser.Dispose();
		}
	}
}
