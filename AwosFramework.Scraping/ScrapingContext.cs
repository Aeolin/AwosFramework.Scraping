using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Routing;
using HtmlAgilityPack;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwosFramework.Scraping
{
	public class ScrapingContext
	{
		public FrozenDictionary<string, string> RouteData { get; init; }
		public FrozenDictionary<string, string> QueryData { get; init; }
		public Uri Url { get; init; }
		public JsonDocument JsonContent { get; init; }
		public HtmlDocument HtmlContent { get; init; }
		public BinaryContent BinaryContent { get; init; }
		public object JobData { get; init; }
	}
}
