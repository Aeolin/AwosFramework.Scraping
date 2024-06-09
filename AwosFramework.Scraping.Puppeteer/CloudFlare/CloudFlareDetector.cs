using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public class CloudFlareDetector : ICloudFlareDetector
	{
		public async Task<ICloudFlareChallenge> DetectCloudFlareAsync(HttpResponseMessage message)
		{
			if (message.Headers.Contains("Cf-Ray"))
			{
				var data = await message.Content.ReadAsStringAsync();
				if (data.Contains("/challenge-platform"))
				{
					return new JavaScriptChallenge(data, message.RequestMessage.RequestUri);
				}
				else
				{
					throw new NotImplementedException();
				}
			}
			else
			{
				return NoChallenge.Instance;
			}
		}
	}
}
