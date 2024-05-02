using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Core.Results
{
	public class ScrapeResult : IScrapeResult
	{
		public IEnumerable<ScrapeJob> Jobs { get; init; }
		public object[] Data { get; init; }
		public bool Failed => false;
		public Exception Exception => null;
		public string ErrorMessage => null;

		public ScrapeResult(IEnumerable<ScrapeJob> jobs, object[] data)
		{
			Jobs = jobs;
			Data = data;
		}
	}
}
