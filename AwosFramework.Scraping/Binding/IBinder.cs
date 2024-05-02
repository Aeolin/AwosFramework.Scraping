using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Binding
{
	public interface IBinder
	{
		public string ParameterName { get; }
		public Type ParameterType { get; }
		public object Bind(ScrapingContext context);
	}
}
