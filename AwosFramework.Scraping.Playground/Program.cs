
using AwosFramework.Scraping.PuppeteerRequestor.CloudFlare;

using (var solver = new CloudFlareSolver())
{
	var detector = new CloudFlareDetector();
	var data = new CloudFlareDataStore();
	var handler = new CloudFlareHandler(detector, solver, data);
	var client = new HttpClient(handler);
	handler.InnerHandler = new HttpClientHandler();

	// should fail
	var response = await client.GetAsync("https://cf-protected-url");

	// should succeed
	response = await client.GetAsync("https://cf-protected-url");
	Console.WriteLine(response.StatusCode);
	Console.WriteLine(await response.Content.ReadAsStringAsync());
}