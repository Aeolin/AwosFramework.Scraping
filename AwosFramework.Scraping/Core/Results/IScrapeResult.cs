using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Core.Results
{
	public interface IScrapeResult
	{
		public IEnumerable<IScrapeJob> Jobs { get; }
		public bool Failed { get; }
		public Exception Exception { get; }
		public string ErrorMessage { get; }
		public object[] Data { get; }
	}
}
