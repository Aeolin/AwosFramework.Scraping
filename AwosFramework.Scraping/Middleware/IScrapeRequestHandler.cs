using AwosFramework.Scraping.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware
{
	public interface IScrapeRequestHandler
	{
		public Task<IScrapeResult> HandleAsync(MiddlewareContext context);
	}
}
