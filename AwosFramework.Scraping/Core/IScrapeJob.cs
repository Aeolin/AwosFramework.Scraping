
namespace AwosFramework.Scraping.Core
{
	public interface IScrapeJob
	{
		Guid Id { get; init; }
		int RetryCount { get; }
		Uri Uri { get; init; }
	}
}