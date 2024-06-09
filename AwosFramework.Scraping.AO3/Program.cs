using AwosFramework.Scraping.Hosting;
using AwosFramework.Scraping.PuppeteerRequestor;
using AwosFramework.Scraping.Hosting.Middleware;
using AwosFramework.Scraping.Hosting.ResultHandlers;
using Microsoft.Extensions.Options;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.AO3;
using Microsoft.Extensions.DependencyInjection;
using AwosFramework.Scraping;

var builder = ScrapeApplication.CreateBuilder(args);
builder.Services.Configure<ScraperConfiguration>(x =>
{ 
	//x.MaxTasks = 1;
	//x.MaxThreads = 1;
});

builder.Services.AddBinderFactory(x => x.AddInbuiltBinders());
builder.Services.AddCloudFlareBypass();
builder.Services.AddHttpRequests();

var app = builder.Build();

app.MapControllers(); 
app.UseHttpRequests();
app.UseDefaultContent();
app.UseRouting();
app.UseControllers();
app.UseJsonResultHandler<WorkMeta>(x => x.WithDirectory("./results").WithBatchSize(1000));

var job = HttpJob.Get("https://archiveofourown.org/works/search?work_search[sort_column]=created_at&work_search[sort_direction]=asc&commit=Search");
app.AddInitialJobs(job);

var result = await app.RunAsync();
app.Dispose();