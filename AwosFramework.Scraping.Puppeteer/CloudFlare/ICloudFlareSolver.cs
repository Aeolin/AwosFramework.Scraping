using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public interface ICloudFlareSolver
	{
		public Task<CloudFlareData> SolveAsync(ICloudFlareChallenge challenge);
	}
}
