using AwosFramework.Scraping.Binding.Attributes;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Core.Results;
using AwosFramework.Scraping.Html;
using AwosFramework.Scraping.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Playground
{
	public class WorldHealthExpoScraper : ScrapeController
	{
		private const string EXHIBITIONER_DETAIL_HANDLER = "EXHIBITIONER_DETAIL_HANDLER";
		private const string EXHIBITIONER_MEMBER_LIST_HANDLER = "EXHIBITIONER_MEMBER_LIST_HANDLER";

		[Route("https://connections.whxevents.com/api/graphql")]
		public IScrapeResult HandleExhibitorList([FromBody(DeserializationType = DeserializationType.Json)] JsonDocument exhibitorView)
		{
			// var array = exhibitorView.RootElement.EnumerateArray().First();
			var exhibitors = exhibitorView.RootElement.GetProperty("data").GetProperty("view").GetProperty("exhibitors");
			string? endCursor = null;
			if (exhibitors.TryGetProperty("pageInfo", out var pageJson) && pageJson.GetProperty("hasNextPage").GetBoolean() && pageJson.TryGetProperty("endCursor", out var endCursorElement))
				endCursor = endCursorElement.GetString();

			var jobs = exhibitors.GetProperty("nodes").EnumerateArray().Select(x => HttpJob.Get(ApiHelper.GetExhibitorDetailRequest(x.GetProperty("id").GetString()!), 1, handlerName: EXHIBITIONER_DETAIL_HANDLER)).ToList();
			if (string.IsNullOrEmpty(endCursor) == false)
				jobs?.Add(HttpJob.Get(ApiHelper.GetExhibitorPageRequest(endCursor), 2));

			return Follow(jobs);
		}

		[HandlerName(EXHIBITIONER_DETAIL_HANDLER)]
		public IScrapeJob HandleExhibitioner([FromBody]JsonDocument detailData)
		{
			var
		}

		[HandlerName(EXHIBITIONER_MEMBER_LIST_HANDLER)]
		public IScrapeJob HandleExhibitionerMembers([FromBody]JsonDocument members, [FromJob]ExhibitorInfo info)
		{
			
		}

		[Route("https://connections.whxevents.com/widget/event/whx-dubai-2026/exhibitor/{exhibitorId}")]
		public IScrapeResult HandleExhibitor(string exhibitorId, [FromCss("#__NEXT_DATA__", DeserializationType = DeserializationType.Json)] JsonDocument document)
		{
			var root = document.RootElement;
			if (root.TryGetProperty("props", out var propsElem) == false || propsElem.TryGetProperty("apolloState", out var apolloElem) == false || apolloElem.TryGetProperty($"Core_Exhibitor:{exhibitorId}", out var exhibitorElem) == false)
				return Fail("No Core_Exhibitor found");

			var lookup = new Dictionary<string, string>();
			foreach (var elem in apolloElem.EnumerateObject())
			{
				var stateType = elem.Name.Split(":", StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries).First();
				var value = stateType switch
				{
					"Core_Location" => elem.Value.GetProperty("name").GetString(),
					"Core_Exhibitor" => elem.Value.GetProperty("name").GetString(),
					"Core_SelectFieldValue" => elem.Value.GetProperty("text").GetString(),
					_ => null
				};

				if (string.IsNullOrEmpty(value) == false)
					lookup[elem.Name] = value;
			}

			var name = GetNestedPropertyString(exhibitorElem, "name");
			var description = GetNestedPropertyString(exhibitorElem, "description");
			var website = GetNestedPropertyString(exhibitorElem, "websiteUrl");
			var type = GetNestedPropertyString(exhibitorElem, "type");
			var country = GetNestedPropertyString(exhibitorElem, "address.country");
			var eventElem = exhibitorElem.EnumerateObject().Where(x => x.Name.StartsWith("withEvent(")).Select(x => x.Value).FirstOrDefault();
			var fields = eventElem.GetProperty("fields");
			var countryCoverage = ResolveRefs(FindFieldValuesElement("Country coverage", fields), lookup).ToArray();
			var medicalEquipment = ResolveRefs(FindFieldValuesElement("Medical Equipment", fields), lookup).ToArray();
			var connectWith = ResolveRefs(FindFieldValuesElement("Interested to connect with", fields), lookup).ToArray();
			var businessNatures = ResolveRefs(FindFieldValuesElement("Nature of Business", fields), lookup).ToArray();
			var booths = ResolveRefs(eventElem.GetProperty("booths"), lookup).ToArray();
			var info = new ExhibitorInfo
			{
				Id = exhibitorId,
				Name = name,
				Description = description,
				Website = website,
				Country = country,
				Type = type,
				Booths = booths,
				ConnectWith = connectWith,
				CountryCoverage = countryCoverage,
				MedicalEquipment = medicalEquipment,
				NatureOfBusiness = businessNatures
			};

			return Ok(info);
		}

		private static string? GetNestedPropertyString(JsonElement element, string path)
		{
			foreach (var segment in path.Split("."))
			{
				if (element.ValueKind != JsonValueKind.Object)
					return null;

				element = element.GetProperty(segment);
			}

			return element.GetString();
		}

		private static JsonElement FindFieldValuesElement(string fieldName, JsonElement eventElement)
		{
			var field = eventElement.EnumerateArray().FirstOrDefault(x => x.GetProperty("name").ValueEquals(fieldName));
			if (field.TryGetProperty("values", out var valuesElem) == false)
				return default;

			return valuesElem;
		}

		private static IEnumerable<string> ResolveRefs(JsonElement values, Dictionary<string, string> lookup)
		{
			if (values.ValueKind != JsonValueKind.Array)
				yield break;

			foreach (var value in values.EnumerateArray())
			{
				var refValue = ResolveRef(value, lookup);
				if (refValue != null)
					yield return refValue;
			}
		}

		private static string? ResolveRef(JsonElement value, Dictionary<string, string> lookup)
		{
			if (value.TryGetProperty("__ref", out var refElement) && refElement.ValueKind == JsonValueKind.String && lookup.TryGetValue(refElement.GetString()!, out var refValue))
				return refValue;

			return null;
		}
	}
}
