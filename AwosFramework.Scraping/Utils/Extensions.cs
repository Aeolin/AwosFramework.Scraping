using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Utils
{
	public static class Extensions
	{
		private static readonly Type[] STRING_TYPE = [typeof(string)];
	public static Func<string, object> GetParseFunction(this Type type)
		{
			if (type == typeof(string))
				return (x) => x;

			if (type == typeof(JsonDocument))
				return (x) => JsonDocument.Parse(x);

			var parseMethod = type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, STRING_TYPE);
			if (parseMethod != null)
				return CreateParseDelegate(parseMethod);

			throw new ArgumentException($"{type.Name} does not declare any Parse function");
			//return (x) => Convert.ChangeType(x, type);
		}

		public static HttpRequestMessage Clone(this HttpRequestMessage req)
		{
			HttpRequestMessage clone = new HttpRequestMessage(req.Method, req.RequestUri);

			// Copy the request's content (via a MemoryStream) into the cloned object
			var ms = new MemoryStream();
			if (req.Content != null)
			{
				req.Content.CopyToAsync(ms, null, CancellationToken.None);
				ms.Position = 0;
				clone.Content = new StreamContent(ms);

				// Copy the content headers
				foreach (var h in req.Content.Headers)
					clone.Content.Headers.Add(h.Key, h.Value);
			}


			clone.Version = req.Version;

			foreach (KeyValuePair<string, object?> option in req.Options)
				clone.Options.Set(new HttpRequestOptionsKey<object?>(option.Key), option.Value);

			foreach (KeyValuePair<string, IEnumerable<string>> header in req.Headers)
				clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

			return clone;
		}

		public static bool HasParseFunction(this Type type)
		{
			return type == typeof(JsonDocument) || type == typeof(string) || type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, STRING_TYPE) != null;
		}

		private static Func<string, object> CreateParseDelegate(MethodInfo parseMethod)
		{
			if (parseMethod == null || !parseMethod.IsStatic || parseMethod.ReturnType == typeof(void))
			{
				throw new ArgumentException("The parse method is not valid.");
			}

			var parameter = Expression.Parameter(typeof(string), "input");

			var methodCall = Expression.Call(parseMethod, parameter);
			var convert = Expression.Convert(methodCall, typeof(object));

			return Expression.Lambda<Func<string, object>>(convert, parameter).Compile();
		}
	}
}
