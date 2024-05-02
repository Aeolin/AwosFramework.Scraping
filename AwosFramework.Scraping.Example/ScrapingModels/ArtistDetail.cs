using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Example.Temp
{
    public class ArtistDescription
    {
        [JsonPropertyName("html")]
        public string HtmlDescription { get; set; }

        [JsonPropertyName("markdown")]
        public string MarkdownDescription { get; set; }
    }

    public class ArtistDetail
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("alternate_names")]
        public string[] AlternateNames { get; set; }

        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("description")]
        public ArtistDescription Description { get; set; }
    }

    public class ArtistInfo
    {
        [JsonPropertyName("artist")]
        public ArtistDetail Artist { get; set; }
    }
}
