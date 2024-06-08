using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public interface ICloudFlareDetector
	{
		public Task<ICloudFlareChallenge> DetectCloudFlareAsync(HttpResponseMessage message);
	}
}
