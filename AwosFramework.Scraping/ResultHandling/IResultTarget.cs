using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.ResultHandling
{
	internal interface IResultTarget
	{
		bool CanHandle(Type type);
		bool Handle(object obj);
	}
}
