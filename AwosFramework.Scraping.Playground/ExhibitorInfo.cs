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
		public string? Id { get; init; }
		public string? Type { get; init; }
		public string? Name { get; init; }
		public string? Website { get; init; }
		public string? Description { get; init; }
		public string? Country { get; init; }
		public string[] CountryCoverage { get; init; }
		public string[] Booths { get; init; }
		public string[] MedicalEquipment { get; init; }
		public string[] ConnectWith { get; set; }
		public string[] NatureOfBusiness { get; set; }
	}
}
