using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Routing
{
	public class SegmentMatcher
	{
		public MatchKind MatchKind { get; init; }
		public string Segment { get; init; }
		public string Key { get; init; }

		private SegmentMatcher(MatchKind kind, string segment, string key)
		{
			MatchKind = kind;
			Segment = segment;
			Key = key;
		}

		public static SegmentMatcher Any() => new SegmentMatcher(MatchKind.Any, null, null);
		public static SegmentMatcher Keyed(string key) => new SegmentMatcher(MatchKind.Keyed, null, key);
		public static SegmentMatcher Exact(string segement) => new SegmentMatcher(MatchKind.Exact, segement.ToLower(), null);

		public bool TryMatch(string segment, out string value)
		{
			switch(MatchKind)
			{
				case MatchKind.Exact:
					value = null;
					return segment.ToLower() == Segment;

				case MatchKind.Any:
					value = null;
					return true;

				case MatchKind.Keyed:
					value = segment;
					return true;

				default:
					value = null;
					return false;
			}
		}

	}
}
