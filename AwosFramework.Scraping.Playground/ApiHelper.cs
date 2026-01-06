using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Playground
{
	public static class ApiHelper
	{
		public static HttpRequestMessage GetExhibitorPageRequest(string? endCursor = null)
		{
			var queryString = FormatQuery(endCursor);
			return new HttpRequestMessage
			{
				Content = new StringContent(queryString, Encoding.UTF8, "application/json"),
				RequestUri = new Uri("https://connections.whxevents.com/api/graphql"),
				Method = HttpMethod.Post
			};
		}


		private static string FormatQuery(string? endCursor = null)
		{
			endCursor = string.IsNullOrEmpty(endCursor) ? "null" : $"\"{endCursor}\"";
			return $$"""
			{
			  "operationName":"EventExhibitorListViewConnectionQuery",
			  "variables":{
			    "withEvent":true,
			    "viewId":"RXZlbnRWaWV3XzEyMjUzMzU==",
			    "eventId":"RXZlbnRfMzAwMDA3NQ==",
					"endCursor": {{endCursor}}
			  },
			  "extensions":{
			    "persistedQuery":{
			      "version":1,
			      "sha256Hash":"b3cb76208b6de3d96c5ba1a8f02e6be6135d5ff1db0a2eecd64b7d15e7e6b5e2"
			    }
			  }
			}
			""";
		}
	}
}
