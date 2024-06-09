using AwosFramework.Scraping.Html.PostProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.AO3
{
	public class CleanNumberAttribute : PostProccesorAttribute<CleanNumberPostProcessor>;
	public class CleanNumberPostProcessor : IPostProcessor
	{
		public string PostProcess(string value) => value.Replace(",", "");
	}
}
