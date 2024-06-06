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
using System.Net.Http;

var builder = ScrapeApplication.CreateBuilder(args);
builder.Services.AddBinderFactory(x => x.AddInbuiltBinders());
builder.Services.AddOptions<DepthBasedScrapingConfig>();
builder.Services.Configure<DepthBasedScrapingConfig>(builder.Configuration.GetSection("ScrapeSettings"));
builder.Services.AddTransient(x => x.GetRequiredService<IOptions<DepthBasedScrapingConfig>>().Value);
builder.Services.AddScoped(x => new HttpClient());
builder.Services.AddHttpClient();

var app = builder.Build();
var cfg = app.Services.GetRequiredService<DepthBasedScrapingConfig>();


app.UseHttpRequests();
app.MapControllers();
app.UseResultHandling();
app.AddJsonResultHandler<ScrapedPage>(x => x.WithDirectory("./pages").WithBatchSize(1000));

var jobs = await app.Services.GetInitialJobsAsync();
app.AddInitialJobs(jobs);

await app.StartAsync();
app.Dispose();