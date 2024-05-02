using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Routing
{
	public struct RouteMatchResult
	{
		public static readonly RouteMatchResult Failed = new RouteMatchResult { Success = false };

		public Uri Url { get; init; }
		public bool Success { get; init; }
		public FrozenDictionary<string, string> Data { get; init; }
	}
}
