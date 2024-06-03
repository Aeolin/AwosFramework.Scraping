using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AwosFramework.Scraping;
using AwosFramework.Scraping.DepthBasedScraper;
using System.Text.Json;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Hosting.Middleware;
using AwosFramework.Scraping.Hosting.ResultHandlers;
using AwosFramework.Scraping.Hosting;
using Microsoft.Extensions.Options;

var builder = ScrapeApplication.CreateBuilder(args);
builder.Services.AddBinderFactory(x => x.AddInbuiltBinders());
builder.Services.AddOptions<DepthBasedScrapingConfig>();
builder.Services.Configure<DepthBasedScrapingConfig>(builder.Configuration.GetSection("ScrapeSettings"));
builder.Services.AddTransient(x => x.GetRequiredService<IOptions<DepthBasedScrapingConfig>>().Value);
builder.Services.AddScoped(x => new HttpClient());

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
	var json = await File.ReadAllTextAsync(cfg.StartUrlsFile);
	var urls = JsonSerializer.Deserialize<string[]>(json);
	jobs.AddRange(urls.Select(x =>HttpJob.Get(x, priority: 10)));
}

app.UseHttpRequests();
app.MapControllers();
app.UseResultHandling();
app.AddJsonResultHandler<ScrapedPage>(x => x.WithDirectory("./pages").WithBatchSize(1000));

app.AddInitialJobs(jobs);

await app.StartAsync();
app.Dispose();