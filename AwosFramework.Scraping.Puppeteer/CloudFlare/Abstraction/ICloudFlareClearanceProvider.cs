
namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare.Abstraction
{
	public interface ICloudFlareClearanceProvider
	{
		Task<CloudFlareClearance> GetCloudFlareClearanceAsync(Uri uri);
	}
}