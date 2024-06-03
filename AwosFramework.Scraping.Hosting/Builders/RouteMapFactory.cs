using AwosFramework.Scraping.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting.Builders
{
	public class RouteMapFactory
	{
		private readonly List<ControllerMethod> _controllerMethods = new List<ControllerMethod>();

		public RouteMapFactory AddControllerMethod(ControllerMethod controllerMethod)
		{
			_controllerMethods.Add(controllerMethod);
			return this;
		}

		public RouteMap Create(IServiceProvider provider) => new RouteMap(_controllerMethods);

	}
}
