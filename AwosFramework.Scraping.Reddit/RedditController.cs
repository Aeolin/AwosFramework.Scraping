using AwosFramework.Factories;
using AwosFramework.Scraping.Binding.Attributes;
using AwosFramework.Scraping.Core;
using AwosFramework.Scraping.Core.Results;
using AwosFramework.Scraping.Html;
using AwosFramework.Scraping.Routing;
using AwosFramework.Scraping.Utils;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Reddit
{
	public class RedditController : ScrapeController
	{
		private readonly ScrapeStats _stats;
		private readonly RedditScraperConfig _config; 

		public RedditController(RedditScraperConfig config, ScrapeStats stats)
		{
			_config = config;
			_stats = stats;
		}

		[Route("https://old.reddit.com/r/{subreddit}/")]
		public IScrapeResult HandlePost(string subreddit, [FromBody(DeserializationType.Html)]PaginatedPosts posts)
		{
			if(_stats.SubCounts.TryGetValue(subreddit, out var subCount) && posts.PostUrls.Length > 0)
			{
				var amount = FastMath.Min(_config.MaxPostsPerSub - subCount.Get(), _config.MaxPosts - _stats.PostCount.Get(), posts.PostUrls.Length);
				if (amount <= 0)
					return Empty();

				var newPosts = posts.PostUrls.Take(amount);
				subCount.Add(amount);
				_stats.PostCount.Add(amount);
				var jobs = newPosts.Select(HttpJob.Get);
				if (string.IsNullOrEmpty(posts.NextUrl) == false && amount == posts.PostUrls.Length)
					jobs = jobs.Append(HttpJob.Get(posts.NextUrl, 10));

				return Follow(jobs);
			}

			return Empty();
		}

		[Route("https://old.reddit.com/r/{subreddit}/comments/{postId}/{postSlug}")]
		public IScrapeResult HandleComments(string subreddit, string postId, string postSlug, [FromBody(DeserializationType.Html)] Post post)
		{
			return Ok(post);
		}

	}
}
