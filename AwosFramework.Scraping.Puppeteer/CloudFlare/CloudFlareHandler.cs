using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public class CloudFlareHandler : DelegatingHandler
	{

		private readonly ICloudFlareDetector _detector;
		private readonly ICloudFlareSolver _solver;
		private readonly CloudFlareDataStore _data;

		public CloudFlareHandler(ICloudFlareDetector detector, ICloudFlareSolver solver, CloudFlareDataStore data)
		{
			_detector=detector;
			_solver=solver;
			_data=data;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (_data.TryGetCloudFlareData(request.RequestUri, out var data))
				data.SetHeaders(request);

			var result = await base.SendAsync(request, cancellationToken);
			if (_data.IsNonCloudFlareDomain(request.RequestUri))
				return result;

			if (result.IsSuccessStatusCode == false)
			{
				var challenge = await _detector.DetectCloudFlareAsync(result);
				if(challenge.Type == ChallengeType.None)
				{
					_data.MarkAsNonCloudFlare(request.RequestUri);
				}
				else
				{
					var solved = await _solver.SolveAsync(challenge, _data);
				}
			}

			return result;
		}
	}
}
