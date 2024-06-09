using AwosFramework.Scraping.PuppeteerRequestor.CloudFlare.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public class CloudFlareHandler : DelegatingHandler
	{

		private readonly ICloudFlareClearanceProvider _clearanceProvider;

		public CloudFlareHandler(ICloudFlareClearanceProvider provider)
		{
			_clearanceProvider = provider;
			//InnerHandler = new HttpClientHandler();
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var clearance = await _clearanceProvider.GetCloudFlareClearanceAsync(request.RequestUri);
			clearance?.SetHeaders(request);
			var result = await base.SendAsync(request, cancellationToken);
			if (result.IsSuccessStatusCode == false)
				_clearanceProvider.ReEvaluateClearance(result);

			return result;
		}
	}
}
