using AwosFramework.Scraping.PuppeteerRequestor.CloudFlare.Abstraction;
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
			if (message.IsSuccessStatusCode == false && message.Headers.Contains("Cf-Ray"))
			{
				var data = await message.Content.ReadAsStringAsync();
				if (data.Contains("/challenge-platform"))
				{
					return new CloudFlareChallenge(message.RequestMessage.RequestUri, ChallengeType.JavaScript);
				}
				else
				{
					throw new NotImplementedException();
				}
			}
			else
			{
				return CloudFlareChallenge.NoChallange;
			}
		}
	}
}
