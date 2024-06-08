using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public record CloudFlareData(Uri Domain, string RayId, string ClearanceToken, string UserAgent)
	{
		public void SetHeaders(HttpRequestMessage message)
		{
			message.Headers.UserAgent.Clear();
			message.Headers.UserAgent.ParseAdd(UserAgent);
			var cookie = $"cf_clearance={ClearanceToken};";
			message.Headers.Add("Cookie", cookie);
		}
	}
}
