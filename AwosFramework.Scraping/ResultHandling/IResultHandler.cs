using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.ResultHandling
{
	public interface IResultHandler : IDisposable
	{
		public Task HandleAsync(object obj);
		public Task SaveAsync();
	}
}
