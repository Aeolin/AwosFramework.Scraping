
namespace AwosFramework.Scraping.Extensions.ServiceCollection
{
	public interface IScraperFactoryBuilder
	{
		ScraperConfiguration Config { get; }

		ScraperFactory AddConfig(ScraperConfiguration config);
		ScraperFactory AddResultHandler(object instance);
		ScraperFactory AddResultHandler<T>();
	}
}