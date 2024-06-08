using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public interface ICloudFlareSolver
	{
		public Task<bool> SolveAsync(ICloudFlareChallenge challenge, CloudFlareDataStore data);
	}
}
