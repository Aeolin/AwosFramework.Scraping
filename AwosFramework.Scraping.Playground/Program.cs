
using AwosFramework.Scraping.PuppeteerRequestor.CloudFlare;
using AwosFramework.Scraping.ResultHandling.Json;
using AwosFramework.Scraping.Utils;
using Microsoft.Extensions.Logging;
using System.Net;

var batchData = new BatchData<string>("Test", 2, 100, "data");
var template = TemplateBuilder.BuildTemplate<BatchData<string>>("{Category}_batch_{BatchNumber:0000}.json");
Console.WriteLine(template(batchData));

var factory = LoggerFactory.Create(x =>
{
	x.AddSimpleConsole(opts =>
	{
		opts.IncludeScopes = true;
		opts.SingleLine = false;
		opts.TimestampFormat = "HH:mm:ss ";
	});
	x.SetMinimumLevel(LogLevel.Debug);
});

using (var solver = new CloudFlareSolver(factory: factory))
{
	var detector = new CloudFlareDetector();
	var clearance = new CloudFlareClearanceProvider(detector, solver);
	var handler = new CloudFlareHandler(clearance);
	var client = new HttpClient(handler);

	// should succeed
	var response = await client.GetAsync("https://nowsecure.nl");
	Console.WriteLine(response.StatusCode);
	Console.WriteLine(await response.Content.ReadAsStringAsync());
}