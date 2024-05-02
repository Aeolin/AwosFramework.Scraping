using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Core.Results;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Formats.Asn1;
using AwosFramework.Scraping.Routing;
using AwosFramework.Scraping.Binding.Attributes;
using System.Collections.Concurrent;
using AwosFramework.Scraping.Example.Models;
using AwosFramework.Scraping.Example.Temp;

namespace AwosFramework.Scraping.Example.Genius
{
	public class GeniusScraper : ScrapeController
	{
		private readonly GeniusConfig _geniusCfg;
		private readonly LastFmConfig _lastFmCfg;
		private readonly ConcurrentDictionary<string, int> _artistCounts;

		public GeniusScraper(GeniusConfig config, LastFmConfig lastFm)
		{
			_geniusCfg = config;
			_lastFmCfg = lastFm;
			if (config.MaxArtistsPerLetter.HasValue)
				_artistCounts = new ConcurrentDictionary<string, int>(config.ArtistLetters.ToDictionary(x => x, x => config.MaxArtistsPerLetter.Value));
		}

		private bool TryGetNextLetter(string currentLetter, out string letter)
		{
			var index = Array.IndexOf(_geniusCfg.ArtistLetters, currentLetter.ToLower());
			if (index != -1 && ++index < _geniusCfg.ArtistLetters.Length)
			{
				letter = _geniusCfg.ArtistLetters[index];
				return true;
			}

			letter = default;
			return false;
		}

		private string GetSongPageRoute(ulong artist, int page) => $"{_geniusCfg.ApiUrl}/artists/{artist}/songs?sort={_geniusCfg.GeniusSongSort}&per_page={_geniusCfg.SongPageSize}&page={page}";


		[Route("https://genius.com/artists-index/{letter}")]
		public IScrapeResult ScrapeArtists(string letter, [FromXPath("*")] PaginatedArtists artists)
		{
			var count = artists.Artists.Count();
			if (_geniusCfg.MaxArtistsPerLetter.HasValue && _artistCounts.TryGetValue(letter, out var left))
			{
				count = count < left ? count : left;
				_artistCounts[letter] = left - count;
			}

			var jobs = artists.Artists.Take(count).Select(x => ScrapeJob.Get(x, 9)).ToList();
			if (TryGetNextLetter(letter, out var nextLetter))
				jobs.Add(ScrapeJob.Get($"{_geniusCfg.Url}/artists-index/{nextLetter}", 10));

			return Follow(jobs);
		}

		[Route("https://genius.com/artists-index/{letter}/all")]
		public IScrapeResult ScrapeAllArtists(string letter, [FromQuery] int page, [FromXPath("*")] PaginatedArtists artists)
		{
			letter = letter.ToLower();
			int count = artists.Artists.Count();
			int countLeft = 0;
			if (_geniusCfg.MaxArtistsPerLetter.HasValue && _artistCounts.TryGetValue(letter, out var left))
			{
				count = count < left ? count : left;
				countLeft = left - count;
				_artistCounts[letter] = countLeft;
			}

			var jobs = artists.Artists.Take(count).Select(x => ScrapeJob.Get(x, 9)).ToList();
			if (artists.NextPage != null && countLeft > 0)
			{
				jobs.Add(ScrapeJob.Get($"{_geniusCfg.Url}{artists.NextPage}", 10));
			}
			else
			{
				if (TryGetNextLetter(letter, out var nextLetter))
					jobs.Add(ScrapeJob.Get($"{_geniusCfg.Url}/artists-index/{nextLetter}/all?page={_geniusCfg.StartPage}", 10));
			}

			return Follow(jobs);
		}

		[Route("https://genius.com/artists/{name}")]
		public IScrapeResult ScrapeArtist(string name, [FromXPath("//meta[@itemprop='page_data']", Attribute = "content")] ArtistInfo info)
		{
			var artist = new Artist
			{
				GeniusId = info.Artist.Id,
				Name = info.Artist.Name,
				Description = info.Artist.Description.MarkdownDescription
			};

			var songs = GetSongPageRoute(artist.GeniusId, 1);
			return OkFollow(ScrapeJob.Get(songs, 8, artist), artist);
		}


		private string TruncateTitle(string title)
		{
			var titleOnly = title;
			var byOffset = titleOnly.LastIndexOf("by");
			if (byOffset != -1)
				titleOnly = titleOnly.Substring(0, byOffset).Trim();

			return titleOnly;
		}

		[Route("https://api.genius.com/artists/*/songs")]
		public IScrapeResult ScrapePaginatedSongs([FromQuery] int page, [FromJob] Artist artist, [FromBody] GeniusReponseModel<SongsModel> response)
		{
			var songs = response.Response.Songs.Where(x => x.PrimaryArtist.Id == artist.GeniusId);
			var nextPage = response.Response.NextPage;
			var songModels = songs.Select(x => new
			{
				x.Url,
				Model = new Song
				{
					GeniusId = x.Id,
					ArtistId = artist.GeniusId,
					PrimaryArtistName = x.PrimaryArtist.Name,
					ArtistNames = x.ArtistNames,
					Title = TruncateTitle(x.Title),
					HasCompleteLyrics = x.LyricsState == "complete",
					FeaturedArtists = x.FeaturedArtists?.Select(x => x.Id)?.ToArray()
				}
			});

			var features = songs.Where(x => x.FeaturedArtists != null && x.FeaturedArtists.Length > 0).SelectMany(x => x.FeaturedArtists.Select(y => new Feature { ArtistId = y.Id, SongId = x.Id }));
			var jobs = songModels.Select(x => ScrapeJob.Get(x.Url, 7, x.Model)).ToList();
			if (nextPage.HasValue && _geniusCfg.MaxSongsPerArtist.HasValue && _geniusCfg.MaxSongsPerArtist.Value < _geniusCfg.SongPageSize * nextPage.Value)
				jobs.Add(ScrapeJob.Get(GetSongPageRoute(artist.GeniusId, nextPage.Value), 8, artist));

			return OkFollow(jobs, features);
		}

		[Route("https://genius.com/{slug}")]
		public IScrapeResult ScrapeLyrics([FromJob] Song song, [FromXPath("//div[@data-lyrics-container='true']")] HtmlNode lyricsNode)
		{
			var builder = new StringBuilder();
			if (lyricsNode != null)
			{
				foreach (var child in lyricsNode.ChildNodes)
				{
					if (child.NodeType == HtmlNodeType.Text)
					{
						builder.Append(child.InnerText);
					}
					else if (child.NodeType == HtmlNodeType.Element)
					{
						switch (child.Name)
						{
							case "br":
								builder.AppendLine();
								break;

							case "a":
								builder.Append(child.InnerText);
								break;
						}
					}
				}
			}

			song.Lyrics = builder.ToString();
			var search = ScrapeJob.Get($"{_lastFmCfg.Url}/de/search?q={song.Title} {song.PrimaryArtistName}", 6, song, true);
			return Follow(search);
		}

		[Route("https://www.last.fm/de/search")]
		public IScrapeResult SearchLastFm([FromJob] Song song, [FromXPath("//td[@class='chartlist-name']/a", Attribute = "href")] string lastFmUrl)
		{
			if (lastFmUrl != null)
				return Follow(ScrapeJob.Get($"{_lastFmCfg.Url}/{lastFmUrl}", 3, song, true));

			return Ok(song);
		}

		[Route("https://www.last.fm/de/music/{artist}/_/{song}")]
		public IScrapeResult ScrapeLastFmTags([FromJob] Song song, [FromXPath("(//ul[contains(@class, 'tags-list')])[1]/li[@class='tag']/a")] IEnumerable<string> tags)
		{
			song.LastFMTags = tags?.ToArray();
			return Ok(song);
		}
	}
}
