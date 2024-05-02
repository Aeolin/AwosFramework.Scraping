using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Example.Temp
{
    public class SongModel
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("artist_names")]
        public string ArtistNames { get; set; }

        [JsonPropertyName("full_title")]
        public string Title { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("lyrics_state")]
        public string LyricsState { get; set; }

        [JsonPropertyName("primary_artist")]
        public SongArtist PrimaryArtist { get; set; }

        [JsonPropertyName("featured_artists")]
        public SongArtist[] FeaturedArtists { get; set; }

        [JsonIgnore]
        public IEnumerable<SongArtist> Artists => FeaturedArtists == null ? [PrimaryArtist] : FeaturedArtists.Concat([PrimaryArtist]);

        [JsonIgnore]
        public string Url => $"https://genius.com{Path}";
    }
}
