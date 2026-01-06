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
			var queryString = FormatExhibitorListQuery(endCursor);
			return new HttpRequestMessage
			{
				Content = new StringContent(queryString, Encoding.UTF8, "application/json"),
				RequestUri = new Uri("https://connections.whxevents.com/api/graphql"),
				Method = HttpMethod.Post
			};
		}

		public static HttpRequestMessage GetExhibitorDetailRequest(string exhibitorId)
		{
			var queryString = FormatExhibitorDetailQuery(exhibitorId);
			return new HttpRequestMessage
			{
				Content = new StringContent(queryString, Encoding.UTF8, "application/json"),
				RequestUri = new Uri("https://connections.whxevents.com/api/graphql"),
				Method = HttpMethod.Post
			};
		}

		public static HttpRequestMessage GetExhibitorMembersRequest(string exhibitorId, string? endCursor = null)
		{
			var queryString = FormatExhibitorMembersQuery(exhibitorId, endCursor);
			return new HttpRequestMessage
			{
				Content = new StringContent(queryString, Encoding.UTF8, "application/json"),
				RequestUri = new Uri("https://connections.whxevents.com/api/graphql"),
				Method = HttpMethod.Post
			};
		}

		private static string FormatExhibitorDetailQuery(string exhibitorId)
		{
			var membersQuery = FormatExhibitorMembersQuery(exhibitorId);
			return $$"""
				[
				  {
				    "operationName":"EventExhibitorDetailsViewQuery",
				    "variables":{
				      "withEvent":true,
				      "skipMeetings":false,
				      "exhibitorId":"{{exhibitorId}}",
				      "eventId":"RXZlbnRfMzAwMDA3NQ=="
				    },
				    "extensions":{
				      "persistedQuery":{
				        "version":1,
				        "sha256Hash":"11891ad980c93f089fb1727527507145684eaffa27463f41cdfc31f2af2f6779"
				      }
				    }
				  },
				  {{membersQuery}}
				]
				""";
		}

		private static string FormatExhibitorMembersQuery(string exhibitorId, string? endCursor = null)
		{
			endCursor = string.IsNullOrEmpty(endCursor) ? "null" : $"\"{endCursor}\"";
			return $$"""
			{
				"operationName":"AllEventExhibitorMembersQuery",
				"variables":{
					"exhibitorId":"{{exhibitorId}}",
					"eventId":"RXZlbnRfMzAwMDA3NQ==",
					"after": {{endCursor}}
				},
				"extensions":{
					"persistedQuery":{
						"version":1,
						"sha256Hash":"77b59a2c5209d9117998e51f7d74d84b9ddf52829add315cd4ffa9a4d7ddf096"
					}
				}
			}
			""";
		}

		private static string FormatExhibitorListQuery(string? endCursor = null)
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
