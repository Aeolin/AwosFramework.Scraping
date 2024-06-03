
using AwosFramework.Scraping.Hosting.Builders;

namespace AwosFramework.Scraping.Hosting
{
    public interface IScraperFactoryBuilder
	{
		ScraperConfiguration Config { get; }

		ScraperFactory AddConfig(ScraperConfiguration config);
		ScraperFactory AddResultHandler(object instance);
		ScraperFactory AddResultHandler<T>();
	}
}