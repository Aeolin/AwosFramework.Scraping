using AwosFramework.Scraping.PuppeteerRequestor.CloudFlare.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public class CloudFlareChallenge : ICloudFlareChallenge
	{
		public static CloudFlareChallenge NoChallange { get; private set; } = new CloudFlareChallenge(null, ChallengeType.None);

		public Uri Url { get; init; }
		public ChallengeType Type { get; init; }

		public CloudFlareChallenge(Uri url, ChallengeType type)
		{
			Url = url;
			Type = type;
		}

	}
}
