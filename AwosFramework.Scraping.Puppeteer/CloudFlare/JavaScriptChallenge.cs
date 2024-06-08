using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public class JavaScriptChallenge : ICloudFlareChallenge
	{
		public string Challenge { get; init; }
		public Uri Domain { get; init; }

		public JavaScriptChallenge(string data, Uri domain)
		{
			this.Challenge = data;
			Domain=domain;
		}

		public ChallengeType Type => ChallengeType.JavaScript;

	}
}
