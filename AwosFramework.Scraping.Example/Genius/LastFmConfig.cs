using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Example.Genius
{
    public class LastFmConfig
    {
        public string Url { get; set; }
        public int MinTagCount { get; set; } = 20;
    }
}
