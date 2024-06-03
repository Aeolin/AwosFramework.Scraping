
using System.Net;

namespace AwosFramework.Scraping.Core
{
	public interface IScrapeJob
	{
		Guid Id { get; init; }
		Uri Uri { get; init; }
		int Priority { get; }
		bool AllowPartialResult { get; }
		public object Data { get; }
		public bool Retry(int maxRetries);
	}
}