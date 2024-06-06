using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Core.Results;
using AwosFramework.Scraping.Hosting;
using AwosFramework.Scraping.Hosting.Builders;
using AwosFramework.Scraping.Middleware;
using AwosFramework.Scraping.ResultHandling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting
{
	public class ScrapeApplication : IHost, IScrapeApplicationBuilder
	{
		public static ScrapeHostApplicationBuilder CreateBuilder(string[] args)
		{
			var builder = new ScrapeHostApplicationBuilder();
			builder.Configuration.AddJsonFile("appsettings.json");
			builder.AddDefaultServices();
			builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
			builder.Logging.AddConsole();
			return builder;
		}

		public IServiceProvider Services { get; init; }
		public MiddlewareCollectionFactory Middleware { get; init; }
		public ResultHandlerCollectionFactory ResultHandlers { get; init; }
		private static Scraper _scraper;
		private readonly List<IScrapeJob> _initialJobs = new List<IScrapeJob>();
		private ILogger _logger;
		private readonly ScraperConfiguration _config;


		public ScrapeApplication(IServiceProvider provider, MiddlewareCollectionFactory middlewareBuilder, ResultHandlerCollectionFactory resultHandlers)
		{
			Services = provider;
			Middleware = middlewareBuilder;
			ResultHandlers = resultHandlers;
			_scraper = Services.GetRequiredService<Scraper>();
			_logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger<ScrapeApplication>();
			_config = provider.GetRequiredService<ScraperConfiguration>();
		}

		private void StartTasks(PriorityQueue<IScrapeJob, int> queue, Dictionary<Task<IScrapeResult>, IScrapeJob> target, ScrapeEngine engine)
		{
			while (target.Count < _config.MaxTasks && queue.Count > 0)
			{
				var job = queue.Dequeue();
				_logger.LogInformation("Begin Scraping {url}", job.Uri);
				target[engine.ScrapeAsync(job)] = job;
			}
		}

		public async Task StartAsync(CancellationToken cancellationToken = default)
		{
			ulong resultCount = 0;
			long totalTicks = 0;
			var watch = new Stopwatch();
			var totalTime = new Stopwatch();
			var jobs = new PriorityQueue<IScrapeJob, int>(_initialJobs.Select(x => (x, x.Priority)));
			var failed = new List<IScrapeJob>();
			var engine = Services.GetRequiredService<ScrapeEngine>();
			var tasks = new Dictionary<Task<IScrapeResult>, IScrapeJob>();
			StartTasks(jobs, tasks, engine);

			watch.Start();
			totalTime.Start();
			while (jobs.Count > 0)
			{
				var completed = await Task.WhenAny(tasks.Keys);
				totalTicks += watch.ElapsedTicks;
				watch.Restart();

				if (resultCount++ % 50 == 0)
				{
					var averageTicks = totalTicks / (double)resultCount;
					var tps = averageTicks / Stopwatch.Frequency * 1000;
					_logger.LogInformation("Average time per scrape: {average}ms", tps);
					var sps = resultCount / (totalTicks / (double)Stopwatch.Frequency);
					Console.Title = $"{_config.ScraperName} | {tps:#.00ms} ms/Job | {sps:#.00} Jobs/s | {resultCount} Results | {jobs.Count} Job Queue | Uptime {totalTime.Elapsed} | ETA {TimeSpan.FromSeconds(jobs.Count/sps)}";
				}

				if (tasks.Remove(completed, out var job))
				{
					if (completed.IsFaulted || completed.Result.Failed)
					{
						_logger.LogError(completed.Exception, "Error scraping {url}", job.Uri);
						if (job.Retry(_config.MaxRetries))
						{
							jobs.Enqueue(job, job.Priority+_config.RetryPriorityPunishment);
						}
						else
						{
							_logger.LogError("Failed to scrape {url} after {retries} retries", job.Uri, _config.MaxRetries);
							failed.Add(job);
						}
					}
					else if (completed.IsCompletedSuccessfully && completed.Result.Failed == false)
					{
						_logger.LogInformation("Scraped {url} successfully", job.Uri);
						var result = completed.Result;
						if (result.Jobs != null)
							foreach (var newJob in result.Jobs)
								jobs.Enqueue(newJob, newJob.Priority);
					}
				}

				StartTasks(jobs, tasks, engine);
			}

			totalTime.Stop();
			watch.Stop();

			_logger.LogInformation("Done scraping, took {timespan}", totalTime.Elapsed);
		}

		public Task StopAsync(CancellationToken cancellationToken = default)
		{
			_scraper.Dispose();
			return Task.CompletedTask;
		}

		public IScrapeApplicationBuilder AddInitialJobs(params IScrapeJob[] jobs)
		{
			_initialJobs.AddRange(jobs);
			return this;
		}

		public IScrapeApplicationBuilder AddInitialJobs(IEnumerable<IScrapeJob> jobs)
		{
			_initialJobs.AddRange(jobs);
			return this;
		}

		public IScrapeApplicationBuilder UseMiddleware(Func<IServiceProvider, IMiddleware> middleware)
		{
			Middleware.AddMiddleware(middleware);
			return this;
		}

		public IScrapeApplicationBuilder UseResultHandler(Func<IServiceProvider, IResultHandler> resultHandlerFactory)
		{
			ResultHandlers.AddResultHandler(resultHandlerFactory);
			return this;
		}

		public void Dispose()
		{
			_scraper?.Dispose();
		}
	}
}
