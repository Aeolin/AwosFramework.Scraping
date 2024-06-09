using AwosFramework.Scraping.PuppeteerRequestor.CloudFlare.Abstraction;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.PuppeteerRequestor.CloudFlare
{
	public class CloudFlareClearanceProvider : ICloudFlareClearanceProvider
	{
		private readonly HashSet<string> _nonCloudFlareDomains = new HashSet<string>();
		private readonly HashSet<string> _unsolvedCloudFlareDomains = new HashSet<string>();
		private readonly ConcurrentDictionary<string, CloudFlareClearance> _cfDomains = new ConcurrentDictionary<string, CloudFlareClearance>();
		private readonly ConcurrentDictionary<string, Task<CloudFlareClearance>> _clearanceTasks = new ConcurrentDictionary<string, Task<CloudFlareClearance>>();
		private readonly ICloudFlareDetector _detector;
		private readonly ICloudFlareSolver _solver;

		public CloudFlareClearanceProvider(ICloudFlareDetector detector, ICloudFlareSolver solver)
		{
			_detector = detector;
			_solver = solver;
		}

		public void ReEvaluateClearance(HttpResponseMessage message)
		{
			var domain = message.RequestMessage.RequestUri.GetLeftPart(UriPartial.Authority);
			if (message.Headers.Contains("Cf-Ray") && message.StatusCode == HttpStatusCode.Forbidden)
			{
				_nonCloudFlareDomains.Remove(domain);
				_cfDomains.TryRemove(domain, out _);
			}
		}

		private bool TryGetClerance(string domain, out CloudFlareClearance clearance)
		{
			if (_cfDomains.TryGetValue(domain, out clearance))
			{
				if (clearance.Expiry < DateTime.Now)
				{
					_cfDomains.Remove(domain, out _);
					_clearanceTasks.Remove(domain, out _);
					return false;
				}

				return true;
			}

			return false;
		}

		public async Task<CloudFlareClearance> GetCloudFlareClearanceAsync(Uri uri)
		{
			var domain = uri.GetLeftPart(UriPartial.Authority);
			if (_nonCloudFlareDomains.Contains(domain))
				return null;

			if (_unsolvedCloudFlareDomains.Contains(domain))
				throw new InvalidOperationException(domain + " is a CloudFlare protected domain but the clearance challenge couldn't be solved.");

			if (TryGetClerance(domain, out var clearance))
				return clearance;

			if (_clearanceTasks.TryGetValue(domain, out var clearanceTask) == false)
			{
				if (_clearanceTasks.TryAdd(domain, SolveCloudFlare(uri)))
				{
					clearanceTask = _clearanceTasks[domain];
					var result = await clearanceTask;
					_clearanceTasks.TryRemove(domain, out _);
					return result;
				}
			}

			return await clearanceTask;
		}

		protected async Task<CloudFlareClearance> SolveCloudFlare(Uri uri)
		{
			var domain = uri.GetLeftPart(UriPartial.Authority);
			var client = new HttpClient();
			var response = await client.GetAsync(uri);
			var challenge = await _detector.DetectCloudFlareAsync(response);
			if (challenge.Type == ChallengeType.None)
			{
				_nonCloudFlareDomains.Add(domain);
				return null;
			}
			else
			{
				var clearance = await _solver.SolveAsync(challenge);
				if (clearance == null)
				{
					_unsolvedCloudFlareDomains.Add(domain);
					return null;
				}
				else
				{
					_cfDomains.TryAdd(domain, clearance);
					return clearance;
				}
			}
		}
	}
}
