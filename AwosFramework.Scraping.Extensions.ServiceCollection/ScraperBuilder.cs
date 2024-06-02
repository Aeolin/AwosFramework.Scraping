using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Extensions.ServiceCollection
{
	public class ScraperFactory : IScraperFactoryBuilder
	{
		public ScraperConfiguration Config { get; private set; }
		private readonly List<object> _resultHandlers = new List<object>();

		public ScraperFactory()
		{
			Config = new ScraperConfiguration();
		}

		public ScraperFactory AddConfig(ScraperConfiguration config)
		{
			Config = config;
			return this;
		}

		public ScraperFactory AddResultHandler<T>()
		{
			_resultHandlers.Add(typeof(T));
			return this;
		}

		public ScraperFactory AddResultHandler(object instance)
		{
			_resultHandlers.Add(instance);
			return this;
		}

		public Scraper Build(IServiceProvider provider)
		{
			var logging = provider.GetRequiredService<ILoggerFactory>();
			var scraper = new Scraper(logging, Config, provider);
			foreach (var handler in _resultHandlers)
				scraper.WithResultHandler(handler);

			return scraper;
		}

	}
}
