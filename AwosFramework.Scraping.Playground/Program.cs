
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
using System.Net.Http.Headers;

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
	cfg.MaxTasks = 4;
	cfg.MaxThreads = 1;
});

builder.Services.AddBinderFactory(x => x.AddInbuiltBinders());
builder.Services.AddHttpClient(Options.DefaultName, client =>
{
	client.DefaultRequestHeaders.Add("User-Agent", "AwosFramework Scraper");
	client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJjb3JlQXBpVXNlcklkIjoiVlhObGNsOHlOekExTnpBMU9RPT0iLCJwZXJtaXNzaW9ucyI6WyJhcHBsaWNhdGlvbjpRWEJ3YkdsallYUnBiMjVmTVRZMk5BPT0iLCJzY2hlbWE6dXNlciJdLCJzZXNzaW9uSWQiOiI2OTVkOGFkOTY2OWE2N2Y0NmIzOGVlMjYiLCJ0eXBlIjoiYWNjZXNzLXRva2VuIiwidXNlcklkIjoiNjk1Y2Y4MzQxNjYzZTg3ZWMzZDQ0YTU2IiwiZW1haWxWZXJpZmllZCI6dHJ1ZSwiaWF0IjoxNzY3NzM4MDg3LCJleHAiOjE3Njc4MjQ0ODcsImlzcyI6ImF1dGgtYXBpIn0.Yeb3u9KXHdGpYvlRomWPgjVqLLnK_CjiAaBCTQdOC40ue-y1gmeiAWqLpLUBgsBmn-GadZgPqT2EjZIS3m8rsb4ChlklpADyd30BT2ElnhtIPnlqA6mmMkxTEq9SH55oulldEtigq1nYpQrJvPfp7mtwAv5o8-SXU-pgrH4cdpT_XGqQ95I5AW7FLxehwGnXeBEhC4HPsj2OCPTZN_4BaLxcGFQ1NBp1flJ4BdeK7zC1CkEOIv_8EgIfKMrz1WYKogcK8s9XA33Cdh02rVPa4C4Q3Ujt7d3aiwby3O9fk1RLbZ9OKEucWtt-8V0fQBZcg-P4IKCj3D2ccUlyZfOz-gpO-4jcZbrSI-FFTR7wyb67V2MpCXj7CPgMGyteOb6bhV6RyzeCg1nmX3pRNTQBcarTVlYHVvAXnxwfAZ1EWbviJpvVxdNQUNNjG3d4zal61iIKdDQPzCuUGh-qslTWTifd6kuqrH8WlNlGLTSarAHt-AcaiuyM6osFDyUrUPQ569SKwZpAk_7_gHDgJZoXlgTIiQ7ma73uqfM5V4Sa__hLXS47Db6NXgz8WwPj_8v7Y9ckEKgHr1ukDqTvlEpct7jWyJhNYeYCXo49ygx8_uPjbv49pOymFR-IWm7N45f0XmKCYwED-w1_mBjvwasQFKycZ5MmMdlyTGdc606KFmQ");
});

builder.Services.AddHttpRequestMiddleware(cfg =>
{
	cfg.WaitOnRateLimit = true;
});

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