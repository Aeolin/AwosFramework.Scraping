using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public class CloudFlareDataStore
	{
		private readonly HashSet<string> _nonCloudFlareDomains = new HashSet<string>();
		private readonly Dictionary<string, CloudFlareData> _cfDomains = new Dictionary<string, CloudFlareData>();

		public bool TryGetCloudFlareData(Uri uri, out CloudFlareData data)
		{
			var domain = uri.GetLeftPart(UriPartial.Authority);
			return _cfDomains.TryGetValue(domain, out data);
		}

		public void MarkAsNonCloudFlare(Uri domain)
		{
			_nonCloudFlareDomains.Add(domain.GetLeftPart(UriPartial.Authority));
		}

		public void SetCloudFlareData(Uri uri, string ray, string clearance, string userAgent)
		{
			var data = new CloudFlareData(uri, ray, clearance, userAgent);
			_cfDomains[uri.GetLeftPart(UriPartial.Authority)] = data;
		}

		public bool IsCloudFlareDomain(Uri domain)
		{
			return _cfDomains.ContainsKey(domain.GetLeftPart(UriPartial.Authority));
		}

		public bool IsNonCloudFlareDomain(Uri domain)
		{
			return _nonCloudFlareDomains.Contains(domain.GetLeftPart(UriPartial.Authority));
		}
	}
}
