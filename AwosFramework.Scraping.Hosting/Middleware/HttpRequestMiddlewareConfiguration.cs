using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting.Middleware
{
	public class HttpRequestMiddlewareConfiguration
	{
		public bool UseHtml { get; set; } = true;
		public bool UseJson { get; set; } = true;
	}
}
