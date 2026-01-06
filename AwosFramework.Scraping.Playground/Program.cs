
using AwosFramework.Scraping;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Hosting;
using AwosFramework.Scraping.Hosting.Middleware;
using AwosFramework.Scraping.Hosting.ResultHandlers;
using AwosFramework.Scraping.Playground;
using AwosFramework.Scraping.PuppeteerRequestor.CloudFlare;
using AwosFramework.Scraping.ResultHandling.Json;
using AwosFramework.Scraping.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

var builder = ScrapeApplication.CreateBuilder(args);
builder.Services.Configure<ScraperConfiguration>(cfg =>
{
	cfg.ScraperName = "Playground Scraper";
	cfg.MaxRetries = 3;
	cfg.MaxTasks = 32;
	cfg.MaxThreads = 4;
});

builder.Services.AddBinderFactory(x => x.AddInbuiltBinders());
builder.Services.AddHttpClient(Options.DefaultName, client =>
{
	client.DefaultRequestHeaders.Add("User-Agent", "AwosFramework Scraper");
});
builder.Services.AddHttpRequestMiddleware();

var app = builder.Build();
app.MapControllers();
app.UseHttpRequests();
app.UseDefaultContent();
app.UseRouting();
app.UseControllers();
app.UseJsonResultHandler<ExhibitorInfo>(x => {
	x.WithDirectory("./results").WithBatchSize(1000);
	x.WithCategory("exhibitor", x => true);
});

app.AddInitialJobs(HttpJob.Get(ApiHelper.GetExhibitorPageRequest()));
await app.RunAsync();