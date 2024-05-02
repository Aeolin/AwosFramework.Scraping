using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.ResultHandling
{
	static class ResultTarget
	{
		public static IResultTarget CreateTarget(Type type, MethodInfo info, object obj, ResultMatchType matchType, ILoggerFactory factory)
		{
			var parameterType = info.GetParameters().First().ParameterType;
			var ctor = typeof(ResultTarget<>).MakeGenericType(parameterType).GetConstructors().First();
			return (IResultTarget)ctor.Invoke([parameterType, info, obj, matchType, factory]);
		}
	}

	internal class ResultTarget<T> : IResultTarget
	{
		private Action<T> _handleAction;
		private Type _type;
		private ResultMatchType _matchType;
		private ILogger _logger;

		public ResultTarget(Type type, MethodInfo method, object instance, ResultMatchType matchType, ILoggerFactory factory)
		{
			_type=type;
			_matchType=matchType;
			_logger = factory.CreateLogger<ResultTarget<T>>();
			if (method.IsStatic)
			{
				_handleAction = method.CreateDelegate<Action<T>>();
			}
			else
			{
				_handleAction = method.CreateDelegate<Action<T>>(instance);
			}
		}

		public bool CanHandle(Type type)
		{
			switch (_matchType)
			{
				case ResultMatchType.Exact:
					return type == _type;

				case ResultMatchType.Assignable:
					return type.IsAssignableTo(_type);

				default:
					return false;
			}
		}

		public bool Handle(object obj)
		{
			try
			{
				var casted = (T)obj;
				_handleAction(casted);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to handle result");
				return false;
			}
		}
	}
}
