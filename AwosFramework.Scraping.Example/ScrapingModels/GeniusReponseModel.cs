using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Example.Temp
{
    public class GeniusReponseModel<T>
    {
        [JsonPropertyName("response")]
        public T Response { get; set; }
    }
}
