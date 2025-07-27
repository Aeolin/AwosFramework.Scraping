using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Reddit
{
	public class RedditScraperConfig
	{
		public int MaxPosts { get; set; }
		public required string[] Subreddits { get; set; }
		public int MaxPostsPerSub { get; set; }
	}
}
