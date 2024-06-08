using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware.Http
{
	public class HttpRequestMiddlewareConfiguration
	{
		public Func<MiddlewareContext, bool> Filter { get; set; }
		public bool CancelMiddlewareOnFilterMismatch { get; set; } = false;
		public bool CancelMiddlewareOnHttpError { get; set; } = true;
	}
}
