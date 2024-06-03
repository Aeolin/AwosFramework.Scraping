using AwosFramework.Scraping.Cli;
using Microsoft.Extensions.DependencyInjection;
using AwosFramework.Scraping.Extensions.ServiceCollection;
using Microsoft.Extensions.Configuration;
using AwosFramework.Scraping;
using AwosFramework.Scraping.DepthBasedScraper;
using System.Text.Json;
using AwosFramework.Scraping.Core;

var builder = ScrapeApplication.CreateBuilder(args);
builder.Services.AddBinderFactory(x => x.AddInbuiltBinders());
builder.Services.AddScrapeControllers();
builder.Services.AddScraper(x => {
	builder.Configuration.GetSection("Scraper").Bind(x.Config);
	
});

builder.Services.AddSingleton(builder.Configuration.GetSection("Settings").Get<DepthBasedScrapingConfig>());

var app = builder.Build();
var cfg = app.Services.GetRequiredService<DepthBasedScrapingConfig>();
var jobs = new List<HttpJob>();

if (cfg.StartUrls != null)
	jobs.AddRange(cfg.StartUrls.Select(HttpJob.Get));

if(cfg.StartUrlsFile?.EndsWith(".txt") ?? false)
{
	var urls = await File.ReadAllLinesAsync(cfg.StartUrlsFile);
	jobs.AddRange(urls.Select(HttpJob.Get));
}

if(cfg.StartUrlsFile?.EndsWith(".json") ?? false)
{
	var urls = await File.ReadAllTextAsync(cfg.StartUrlsFile);
	var json = JsonSerializer.Deserialize<string[]>(urls);
	jobs.AddRange(json.Select(HttpJob.Get));
}

app.AddJobs(jobs);

await app.StartAsync();
app.Dispose();