using AwosFramework.Scraping.ResultHandling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware.Result
{
	public class ResultHandlerCollection : ReadOnlyCollection<IResultHandler>
	{
		public ResultHandlerCollection(IList<IResultHandler> list) : base(list)
		{
		}
	}
}
