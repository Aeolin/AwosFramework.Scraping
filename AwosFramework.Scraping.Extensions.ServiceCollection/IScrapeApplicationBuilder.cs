using AwosFramework.Scraping.ResultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Extensions.ServiceCollection
{
	public interface IScrapeApplicationBuilder
	{
		public IScrapeApplicationBuilder UseMiddleware(object middleware);
		public IScrapeApplicationBuilder UseResultHandler(object resultHandle);
	}
}
