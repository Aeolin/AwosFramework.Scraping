using AwosFramework.Scraping;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Hosting;
using AwosFramework.Scraping.Hosting.Middleware;
using AwosFramework.Scraping.Hosting.ResultHandlers;
using AwosFramework.Scraping.Reddit;
using AwosFramework.Scraping.ResultHandling.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = ScrapeApplication.CreateBuilder(args);
builder.Services.Configure<ScraperConfiguration>(cfg =>
{
	cfg.ScraperName = "Reddit Scraper";
	cfg.MaxRetries = 3;
	cfg.MaxTasks = 4;
	cfg.MaxThreads = 1;
});

var cfg = builder.Configuration.GetSection("Reddit").Get<RedditScraperConfig>();
ArgumentNullException.ThrowIfNull(cfg);
builder.Services.AddSingleton(cfg);
builder.Services.AddBinderFactory(x => x.AddInbuiltBinders());
builder.Services.AddHttpClient(Options.DefaultName, client =>
{
	client.DefaultRequestHeaders.Add("User-Agent", "AwosFramework Reddit Scraper");
});
builder.Services.AddHttpRequestMiddleware();
builder.Services.AddSingleton<ScrapeStats>();

var app = builder.Build();
app.MapControllers();
app.UseHttpRequests();
app.UseDefaultContent();
app.UseRouting();
app.UseControllers();
app.UseJsonResultHandler<Post>(x => {
	x.WithDirectory("./results").WithBatchSize(100);
	foreach (var subreddit in cfg.Subreddits)
	{
		x.WithCategory(subreddit, x => x.Subreddit == subreddit, cat =>
		{
			cat.WithFileNameTemplate($"{subreddit}_batch_{{{nameof(BatchData<Post>.BatchNumber)}:00}}.json");
		});
	}
});

var jobs = cfg.Subreddits.Select(x => HttpJob.Get($"https://old.reddit.com/r/{x}"));
app.AddInitialJobs(jobs);

await app.StartAsync();
app.Dispose();