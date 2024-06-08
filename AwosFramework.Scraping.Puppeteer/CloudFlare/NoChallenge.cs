using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public class NoChallenge : ICloudFlareChallenge
	{
		public static NoChallenge Instance { get; private set; } = new NoChallenge();

		private NoChallenge()
		{

		}

		public ChallengeType Type => ChallengeType.None;
	}
}
