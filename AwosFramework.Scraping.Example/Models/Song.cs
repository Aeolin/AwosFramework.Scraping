using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Example.Models
{
	public class Song
	{
		public ulong GeniusId { get; set; }
		public ulong ArtistId { get; set; }
		public string PrimaryArtistName { get; set; }
		public ulong[] FeaturedArtists { get; set; }
		public string Title { get; set; }
		public string Lyrics { get; set; }
		public string ArtistNames { get; set; }
		public bool HasCompleteLyrics { get; set; }
		public string[] LastFMTags { get; set; }
	}
}
