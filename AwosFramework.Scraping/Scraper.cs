using AwosFramework.Multithreading.Runners;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Core.Results;
using AwosFramework.Scraping.ResultHandling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwosFramework.Scraping
{
	public class Scraper : IDisposable
	{
		private readonly ILogger _logger;
		private readonly IServiceProvider _container;
		private readonly ScraperConfiguration _config;
		private readonly RunnerGroup<ScrapeJob, IScrapeResult, ScrapeEngine> _runners;
		private readonly ResultTargetRegistry _resultHandlers;
		private readonly List<TaskCompletionSource> _doneAwaiters = new List<TaskCompletionSource>();
		private int _jobInWorkCount = 0;

		public Scraper(ILoggerFactory loggerFactory, ScraperConfiguration config, IServiceProvider container)
		{
			_logger = loggerFactory.CreateLogger<Scraper>();
			_runners = new RunnerGroup<ScrapeJob, IScrapeResult, ScrapeEngine>(config.ScraperName ?? Assembly.GetExecutingAssembly().GetName().Name + " Scraping Runner", ScrapeLogic);
			_runners.OnError += _runners_OnError;
			_runners.OnResult += _runners_OnResult;
			_config = config;
			_container = container;
			_resultHandlers = new ResultTargetRegistry(loggerFactory);
		}

		private IScrapeResult ScrapeLogic(ScrapeEngine engine, ScrapeJob job)
		{
			try
			{
				Interlocked.Increment(ref _jobInWorkCount);
				var task = engine.ScrapeAsync(job);
				if (task == null)
					return null;

				var result = task.Result;

				if (result.Failed == false)
				{
					if (result.Data != null)
						foreach (var item in result.Data)
							_resultHandlers.HandleResult(item);

					if (result.Jobs != null)
						_runners.QueueJobs(result.Jobs.Select(x => (x, x.Priority)));
				}
				else
				{
					if (job.AllowPartialResult && result.Data != null)
						foreach (var item in result.Data)
							_resultHandlers.HandleResult(item);

					if (result.Exception != null)
						_logger.LogError(result.Exception, "Error while scraping {job}", job);
					else
						_logger.LogError(result.ErrorMessage);
				}

				return result;
			}
			finally
			{
				Interlocked.Decrement(ref _jobInWorkCount);
			}
		}

		private void FireDone()
		{
			lock (_doneAwaiters)
			{
				_doneAwaiters.ForEach(x => x.SetResult());
				_doneAwaiters.Clear();
			}
		}

		public Scraper WithResultHandler(object obj)
		{
			_resultHandlers.RegisterHandler(obj);
			return this;
		}

		public Scraper WithResultHandler<T>()
		{
			_resultHandlers.RegisterHandler(typeof(T));
			return this;
		}

		private void _runners_OnResult(object sender, IScrapeResult result)
		{
			var count = _runners.Jobs.Count;
			Debug.WriteLine($"Job queue: {_runners.Jobs.Count}");

			if (count == 0 && _jobInWorkCount == 0)
				FireDone();
		}

		private void _runners_OnError(object source, ScrapeJob input, Exception ex)
		{
			if (input.RetryCount < _config.MaxRetries)
			{
				input.Retry();
				_runners.QueueJob(input);
			}
			else if (_runners.Jobs.Count == 0 && _jobInWorkCount == 0)
			{
				FireDone();
			}
		}

		public async Task RunAsync(ScrapeJob initialJob)
		{
			for (int i = 0; i < _config.MaxThreads; i++)
			{
				var engine = _container.GetService<ScrapeEngine>();
				if (engine == null)
					throw new InvalidOperationException($"Service for {nameof(ScrapeEngine)} could not be resolved");

				new Runner<ScrapeJob, IScrapeResult, ScrapeEngine>(_runners, engine);
			}

			_runners.StartAll();
			_runners.QueueJob(initialJob);

			var source = new TaskCompletionSource();
			_doneAwaiters.Add(source);
			await source.Task;
			_resultHandlers.SaveAll();
			_resultHandlers.Dispose();
		}

		public void Dispose()
		{
			_runners.StopAll();
			_runners.Dispose();
		}
	}
}
