using PuppeteerSharp;

namespace AwosFramework.Scraping.PuppeteerRequestor
{
	public interface IBrowserPool
	{
		Task<IBrowser> GetBrowserAsync();
		Task ReturnBrowserAsync(IBrowser browser);
	}
}