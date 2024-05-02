using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Example.Temp
{
    public class LastFmResult
    {
        [JsonPropertyName("toptags")]
        public TopTags TopTags { get; set; }

        [JsonPropertyName("error")]
        public int Error { get; set; }
    }
}
