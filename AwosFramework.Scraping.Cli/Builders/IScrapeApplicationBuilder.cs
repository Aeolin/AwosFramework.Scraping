using AwosFramework.Scraping.Middleware;
using AwosFramework.Scraping.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting.Builders
{
	public interface IScrapeApplicationBuilder
	{
		public IScrapeApplicationBuilder UseMiddleware(Func<IServiceProvider, IMiddleware> middlewareFactory);
		public IScrapeApplicationBuilder UseResultHandler(object resultHandle);
	}
}
