using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public record CloudFlareData(Uri Domain, string RayId, string Cookie, string UserAgent)
	{
		public void SetHeaders(HttpRequestMessage message)
		{
			message.Headers.UserAgent.Clear();
			message.Headers.UserAgent.ParseAdd(UserAgent);
			message.Headers.Add("Cookie", Cookie);
		}
	}
}
