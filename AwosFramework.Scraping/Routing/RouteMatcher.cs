using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Routing
{
	public class RouteMatcher
	{
		private readonly SegmentMatcher[] _matchers;
		private readonly string _host;
		public string[] RouteKeywords { get; init; }

		public RouteMatcher(string host, string route)
		{
			_host = host;
			if (string.IsNullOrEmpty(route))
			{
				_matchers = Array.Empty<SegmentMatcher>();
			}
			else
			{
				var segments = route.TrimStart('/').Split('/');
				_matchers = new SegmentMatcher[segments.Length];
				for (int i = 0; i < segments.Length; i++)
				{
					var segment = segments[i];
					if (segment == "*")
					{
						_matchers[i] = SegmentMatcher.Any();
					}
					else if (segment.StartsWith("{") && segment.EndsWith("}"))
					{
						var key = segment[1..^1];
						_matchers[i] = SegmentMatcher.Keyed(key);
					}
					else
					{
						_matchers[i] = SegmentMatcher.Exact(segment);
					}
				}
			}

			_host=host;
			RouteKeywords = _matchers.Where(x => x.MatchKind == MatchKind.Keyed).Select(x => x.Key).ToArray();
		}

		public RouteMatchResult Match(string route) => Match(new Uri(route));

		public RouteMatchResult Match(Uri url)
		{
			var segments = url.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
			if (segments.Length != _matchers.Length || (_host != null && $"{url.Scheme}://{url.Host}" != _host))
				return RouteMatchResult.Failed;

			var dict = new Dictionary<string, string>();
			for (int i = 0; i < _matchers.Length; i++)
			{
				var matcher = _matchers[i];
				var segment = segments[i];
				if (matcher.TryMatch(segment, out var value) == false)
				{
					return RouteMatchResult.Failed;
				}

				if (matcher.MatchKind == MatchKind.Keyed)
					dict[matcher.Key] = value;
			}

			return new RouteMatchResult { Success = true, Data = dict.ToFrozenDictionary(), Url = url };
		}
	}
}
