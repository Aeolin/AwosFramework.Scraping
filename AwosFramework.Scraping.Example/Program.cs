using AwosFramework.Scraping;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Example;
using AwosFramework.Scraping.Example.Genius;
using AwosFramework.Scraping.Example.Models;
using AwosFramework.Scraping.Extensions.ReInject;
using AwosFramework.Scraping.ResultHandling.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReInject;
using ReInject.Interfaces;
using System.Diagnostics;
using System.Net.Http.Headers;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var geniusCfg = config.GetSection("GeniusSettings").Get<GeniusConfig>();

var container = Injector.GetContainer();
container.AddTransient<HttpClient>(() =>
{
	var client = new HttpClient();
	client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", geniusCfg.ApiKey);
	return client;
});

var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole().AddConfiguration(config.GetSection("Logging")); });

container.AddSingleton<IConfiguration>(config);
container.AddSingleton<IDependencyContainer>(container);
container.AddSingleton<System.IServiceProvider>(container);
container.AddSingleton<ILoggerFactory>(loggerFactory);
container.AddSingleton<ScraperConfiguration>(config.GetSection("ScrapingSettings").Get<ScraperConfiguration>());
container.AddSingleton<LastFmConfig>(config.GetSection("LastFmSettings").Get<LastFmConfig>());
container.AddSingleton<GeniusConfig>(geniusCfg);
container.AddDefaultBinders();
container.MapScrapeControllers();

var output = config.GetSection("Output").Get<OutputSettings>();
var songHandler = new JsonResultHandler<Song>($"{output.Path}/songs", output.SaveBatchSize);
var artistHandler = new JsonResultHandler<Artist>($"{output.Path}/artists", output.SaveBatchSize);
var featureHandler = new JsonResultHandler<Feature>($"{output.Path}/features", output.SaveBatchSize);
var taggedSongHandler = new JsonResultHandler<Song>($"{output.Path}/tagged-songs", output.SaveBatchSize, filter: (x) => x.LastFMTags != null);

using var scraper = new Scraper(loggerFactory, container.GetInstance<ScraperConfiguration>(), container)
	.WithResultHandler(songHandler)
	.WithResultHandler(artistHandler)
	.WithResultHandler(featureHandler)
	.WithResultHandler(taggedSongHandler);


if (geniusCfg.ArtistLetters.Length == 0)
{
	Console.WriteLine($"Nothing todo, no letters found {geniusCfg.ArtistLetters.Length}");
	return;
}

var initialUrl = $"{geniusCfg.Url}/artists-index/{geniusCfg.ArtistLetters.First()}";
if (geniusCfg.OnlyTopArtists == false)
	initialUrl += $"/all?page={geniusCfg.StartPage}";


await scraper.RunAsync(ScrapeJob.Get(initialUrl));
Console.WriteLine($"Done");