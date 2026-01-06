
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
	cfg.MaxTasks = 32;
	cfg.MaxThreads = 4;
});

builder.Services.AddBinderFactory(x => x.AddInbuiltBinders());
builder.Services.AddHttpClient(Options.DefaultName, client =>
{
	client.DefaultRequestHeaders.Add("User-Agent", "AwosFramework Scraper");
	client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJjb3JlQXBpVXNlcklkIjoiVlhObGNsOHlOekExTnpBMU9RPT0iLCJwZXJtaXNzaW9ucyI6WyJhcHBsaWNhdGlvbjpRWEJ3YkdsallYUnBiMjVmTVRZMk5BPT0iLCJzY2hlbWE6dXNlciJdLCJzZXNzaW9uSWQiOiI2OTVkM2MwNDE4MmQ3ODQ0YjNmNDliN2IiLCJ0eXBlIjoiYWNjZXNzLXRva2VuIiwidXNlcklkIjoiNjk1Y2Y4MzQxNjYzZTg3ZWMzZDQ0YTU2IiwiZW1haWxWZXJpZmllZCI6dHJ1ZSwiaWF0IjoxNzY3NzE3OTIzLCJleHAiOjE3Njc4MDQzMjMsImlzcyI6ImF1dGgtYXBpIn0.fGLcaGKb1fhUBiY-94WuQQvZY1JEaJHZCHpjA4kcvyunJnk1QQKQJRXo7cDrNPcbRf7ZDojquSBvRzwsZCpLf2nznVuQhC5JZajmBgMT1yWOu9_SaNsn43o2h_0K9x0wn2Qc00Ct0V5mgB9qAmgoElrfl9vPICOr1WGUCG9ILA1tSm3gu2zuOKah6s7pzf45AWZfHb4HJcNqR4UktsOkj0I7hwQbcP7Oo3098J7mHE1lc_NlOY2dhtysj9fL8L_Y-Gj6JV30fbTgP2polCk1uVCNxbU06-ibKW7hJEhQk_Yi9Um7lhhHuBic6uammPPPgch5rDdKEwg8q3E5TqUc92E1yLH4yWxKqptZ5_Sr-4xQUvah8Vogh_Px2u_WjmceSeYCMgjFcrGSgo3ndTcYQtdqAaCqOwr8DSXlYoudF-BYL81IcTYMLAjNoyJ7NsME6gCUFkpat4QMatF5slclniznJM8TevtCd0f--ELUxQ5Ib85utaoBNYarKAUkrylWJUk_TIP_sfcCwcoQjNe48puQU_-PHnwEtOFJtkBjMdWalA3734W0eDk3a7qRRS_5CYR9tnYV3qWEfSyK-H1ZIwW-qgWtpM2suHRKunyCY63oX9QbABspYVMFShoMPibADhR_nuDYvEJbm6by7yynHg9waurOnbkQt7En64VIW54");
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