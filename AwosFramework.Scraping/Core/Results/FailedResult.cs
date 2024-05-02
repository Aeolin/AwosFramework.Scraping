using AwosFramework.Scraping.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Core.Results
{
	public class FailedResult : IScrapeResult
	{
		public IEnumerable<ScrapeJob> Jobs => Enumerable.Empty<ScrapeJob>();

		public bool Failed => true;

		public Exception Exception { get; init; }

		public object[] Data => null;

		public string ErrorMessage { get; init; }

		public override string ToString()
		{
			return ErrorMessage ?? Exception?.ToString() ?? "Unknown reason";
		}

		public FailedResult(Exception exception, string message = null)
		{
			Exception = exception;
			ErrorMessage = message;
		}

		public FailedResult(string message = null)
		{
			ErrorMessage = message;
		}

	}
}
