using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Example.Genius
{
	public class GeniusConfig
	{
		public int? MaxSongsPerArtist { get; set; }
		public int? MaxArtistsPerLetter { get; set; }
		public string ApiUrl { get; set; } = "https://api.genius.com";
		public string ApiKey { get; set; }
		public string Url { get; set; } = "https://genius.com";
		public string GeniusSongSort { get; set; } = "popularity";
		public string[] ArtistLetters { get; set; }
		public int StartPage { get; set; } = 1;
		public bool OnlyTopArtists { get; set; } = false;

		[Range(1, 50)]
		public int SongPageSize { get; set; } = 50;
	}
}
