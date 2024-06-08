using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Hosting.Builders;
using AwosFramework.Scraping.Middleware;
using AwosFramework.Scraping.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting
{
	public interface IScrapeApplicationBuilder
	{
		public MiddlewareCollectionFactory Middleware { get; }
		public ResultHandlerCollectionFactory ResultHandlers { get; }
		public IServiceProvider Services { get; }

		public IScrapeApplicationBuilder AddInitialJobs(params IScrapeJob[] jobs);
		public IScrapeApplicationBuilder AddInitialJobs(IEnumerable<IScrapeJob> jobs);
	}
}
