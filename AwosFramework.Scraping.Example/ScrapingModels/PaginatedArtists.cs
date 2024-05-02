using AwosFramework.Scraping.Html.XPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Example.Temp
{
    public class PaginatedArtists
    {
        [XPath("//ul[@class='artists_index_list']/li/a", Attribute = "href")]
        public IEnumerable<string> Artists { get; set; }


        [XPath("//div[@class='pagination']/a[@rel='next']", Attribute = "href")]
        public string NextPage { get; set; }
    }
}
