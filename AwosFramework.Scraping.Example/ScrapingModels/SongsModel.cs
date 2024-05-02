using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Example.Temp
{
    public class SongsModel
    {
        [JsonPropertyName("songs")]
        public SongModel[] Songs { get; set; }

        [JsonPropertyName("next_page")]
        public int? NextPage { get; set; }
    }
}
