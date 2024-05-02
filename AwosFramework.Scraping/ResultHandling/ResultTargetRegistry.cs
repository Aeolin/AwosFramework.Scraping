using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.ResultHandling
{
	internal class ResultTargetRegistry : IDisposable
	{
		private readonly List<IResultTarget> _handlers = new List<IResultTarget>();
		private ILoggerFactory _factory;

		public ResultTargetRegistry(ILoggerFactory factory)
		{
			_factory=factory;
		}

		public void SaveAll()
		{
			foreach (var handler in _handlers.OfType<IResultHandler>())
				handler.Save();
		}

		public void Dispose()
		{
			foreach (var handler in _handlers.OfType<IDisposable>())
				handler.Dispose();

			_handlers.Clear();
		}

		public bool HandleResult(object obj)
		{
			if (obj == null)
				return false;

			bool result = true;
			foreach (var handler in _handlers)
			{
				if (handler.CanHandle(obj.GetType()))
					result &= handler.Handle(obj);
			}

			return result;
		}

		public void RegisterHandler(Type type)
		{
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			foreach (var method in methods)
			{
				var attr = method.GetCustomAttribute<HandleResultAttribute>();
				var parameters = method.GetParameters();
				if (attr != null && parameters.Length == 1)
				{
					var handler = ResultTarget.CreateTarget(type, method, null, attr.MatchType, _factory);
					_handlers.Add(handler);
				}
			}
		}

		public void RegisterHandler(object obj)
		{
			var type = obj.GetType();
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
			foreach (var method in methods)
			{
				var attr = method.GetCustomAttribute<HandleResultAttribute>();
				var parameters = method.GetParameters();
				if (attr != null && parameters.Length == 1)
				{
					var handler = ResultTarget.CreateTarget(type, method, obj, attr.MatchType, _factory);
					_handlers.Add(handler);
				}
			}
		}
	}
}
