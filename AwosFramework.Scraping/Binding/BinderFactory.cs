using AwosFramework.Scraping.Binding.Attributes;
using AwosFramework.Scraping.Html.Css;
using AwosFramework.Scraping.Html.XPath;
using AwosFramework.Scraping.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AwosFramework.Scraping.Routing;

namespace AwosFramework.Scraping.Binding
{
	public class BinderFactory : IBinderFactory
	{
		private readonly List<IBinderGenerator> _generators;

		public BinderFactory(IEnumerable<IBinderGenerator> generators = null)
		{
			_generators = generators?.ToList() ?? new List<IBinderGenerator>();
		}

		public void AddBinderGenerator(IBinderGenerator generator)
		{
			_generators.Add(generator);
		}


		public IBinder CreateBinder(ParameterInfo parameter, RouteMatcher matcher, object defaultValue)
		{
			foreach (var generator in _generators)
				if (generator.TryCreateBinder(parameter, matcher, defaultValue, out var binder))
					return binder;

			var key = parameter.Name.ToLower();
			if (matcher.RouteKeywords.Contains(key) == false)
				throw new InvalidOperationException($"Route parameter {key} not found in route");

			return new RouteValueBinder(key, parameter.ParameterType, defaultValue);
		}
	}
}
