using AwosFramework.Scraping.Hosting.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting.ResultHandlers
{
	public static class Extensions
	{
		public static IScrapeApplicationBuilder UseJsonResultHandler<T>(this IScrapeApplicationBuilder scrapingBuilder, Action<JsonResultHandlerBuilder<T>> configure)
		{
			var builder = new JsonResultHandlerBuilder<T>();
			configure?.Invoke(builder);
			scrapingBuilder.UseResultHandler(builder.Build());
			return scrapingBuilder;
		}
	}
}
