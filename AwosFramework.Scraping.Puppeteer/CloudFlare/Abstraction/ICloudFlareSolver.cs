using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare.Abstraction
{
	public interface ICloudFlareSolver
	{
		public Task<CloudFlareClearance> SolveAsync(ICloudFlareChallenge challenge);
	}
}
