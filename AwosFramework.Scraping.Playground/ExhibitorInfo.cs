using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Playground
{
	public class ExhibitorInfo
	{
		[JsonPropertyName("id")]
		public string? Id { get; set; }

		[JsonPropertyName("type")]
		public string? Type { get; set; }

		[JsonPropertyName("name")]
		public string? Name { get; set; }

		[JsonPropertyName("email")]
		public string? Email { get; set; }

		[JsonPropertyName("websiteUrl")]
		public string? Website { get; set; }

		[JsonPropertyName("description")]
		public string? Description { get; set; }

		public string? Country { get; set; }
		public string[] CountryCoverage { get; set; }
		public string[] Booths { get; set; }
		public string[] MedicalEquipment { get; set; }
		public string[] ConnectWith { get; set; }
		public string[] NatureOfBusiness { get; set; }
		public List<ExhibitorMember> Members { get; set; }
	}
}
