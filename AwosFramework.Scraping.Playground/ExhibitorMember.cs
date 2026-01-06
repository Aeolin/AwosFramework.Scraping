using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Playground
{
	public class ExhibitorMember
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("firstName")]
		public string FirstName { get; set; }
		
		[JsonPropertyName("lastName")]
		public string LastName { get; set; }

		[JsonPropertyName("organization")]
		public string Organization { get; set; }

		[JsonPropertyName("jobTitle")]
		public string JobTitle { get; set; }
	}	
}
