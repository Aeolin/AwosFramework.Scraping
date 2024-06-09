using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html.PostProcessing
{
	public interface IPostProcessor
	{
		public string PostProcess(string value);
	}
}
