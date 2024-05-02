using AwosFramework.Scraping.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Core
{
	public interface IScrapeRoute
	{
		bool CanHandle(ScrapeJob job);
		Task<IScrapeResult> HandleAsync(ScrapeController controller, ScrapeJob job);
		Type ControllerType { get; }
	}
}
