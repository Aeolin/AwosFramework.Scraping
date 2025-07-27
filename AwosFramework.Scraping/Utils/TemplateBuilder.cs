using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AwosFramework.Scraping.Utils
{
	public delegate string NameTemplate<T>(T value);

	public record TemplateVariable(int Index, int Length, string Expression, string Format);

	public static class TemplateBuilder
	{
		private static readonly Regex VariablePattern = new Regex(@"\{([^\s:}]+)(:([^\}]+))?\}", RegexOptions.Compiled);
		private static readonly MethodInfo StringBuilderAppend = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), BindingFlags.Public|BindingFlags.Instance, [typeof(string)]);

		public static NameTemplate<T> BuildTemplate<T>(string template)
		{
			var templateVariables = ParseTemplateVariables(template);

			if (templateVariables.Length == 0)
			{
				// Fast exit: no variables, just return the template string
				var parameter = Expression.Parameter(typeof(T), "value");
				var returnTemplate = Expression.Constant(template);
				var expression = Expression.Lambda<NameTemplate<T>>(returnTemplate, parameter);
				return expression.Compile();
			}

			return BuildTemplateWithVariables<T>(template, templateVariables);
		}

		private static TemplateVariable[] ParseTemplateVariables(string template)
		{
			return VariablePattern.Matches(template)
					.Select(match => new TemplateVariable(
							match.Index,
							match.Value.Length,
							match.Groups[1].Value,
							match.Groups[3].Value))
					.ToArray();
		}

		private static NameTemplate<T> BuildTemplateWithVariables<T>(string template, TemplateVariable[] templateVariables)
		{
			var parameter = Expression.Parameter(typeof(T), "value");
			var stringBuilderType = typeof(StringBuilder);
			var stringBuilderVariable = Expression.Variable(stringBuilderType, "stringBuilder");
			var stringBuilderConstructor = Expression.New(stringBuilderType);

			var blockExpressions = new List<Expression>
			{
					Expression.Assign(stringBuilderVariable, stringBuilderConstructor)
			};

			int lastIndex = 0;
			foreach (var templateVariable in templateVariables)
			{
				// Append static text before the variable
				if (templateVariable.Index > lastIndex)
				{
					string staticText = template.Substring(lastIndex, templateVariable.Index - lastIndex);
					blockExpressions.Add(
							Expression.Call(
								stringBuilderVariable,
								StringBuilderAppend,
								Expression.Constant(staticText))
					);
				}

				// Append variable value
				blockExpressions.Add(BuildVariableAppendExpression(parameter, stringBuilderVariable, templateVariable));

				lastIndex = templateVariable.Index + templateVariable.Length;
			}

			// Append any remaining static text
			if (lastIndex < template.Length)
			{
				string staticText = template.Substring(lastIndex);
				blockExpressions.Add(
						Expression.Call(stringBuilderVariable, StringBuilderAppend, Expression.Constant(staticText))
				);
			}

			// Return stringBuilder.ToString()
			blockExpressions.Add(Expression.Call(stringBuilderVariable, nameof(StringBuilder.ToString), null));

			var body = Expression.Block(new[] { stringBuilderVariable }, blockExpressions);
			var lambda = Expression.Lambda<NameTemplate<T>>(body, parameter);
			return lambda.Compile();
		}

		private static Expression BuildVariableAppendExpression(
				ParameterExpression parameter,
				ParameterExpression stringBuilderVariable,
				TemplateVariable templateVariable)
		{
			var valueExpression = BuildVariableAccessExpression(parameter, templateVariable.Expression);
			var valueVariable = Expression.Variable(valueExpression.Type, "valueVariable");
			var assignValue = Expression.Assign(valueVariable, valueExpression);

			var toStringMethod = valueExpression.Type.GetMethod("ToString", Type.EmptyTypes);
			var toStringWithFormatMethod = valueExpression.Type.GetMethod("ToString", new[] { typeof(string) });

			Expression appendExpression;
			if (toStringWithFormatMethod != null && !string.IsNullOrEmpty(templateVariable.Format))
			{
				appendExpression = Expression.Call(
						stringBuilderVariable,
						StringBuilderAppend,
						Expression.Call(valueVariable, toStringWithFormatMethod, Expression.Constant(templateVariable.Format!))
				);
			}
			else
			{
				appendExpression = Expression.Call(
						stringBuilderVariable,
						StringBuilderAppend,
						Expression.Call(valueVariable, toStringMethod!)
				);
			}

			if (valueExpression.Type.IsValueType && Nullable.GetUnderlyingType(valueExpression.Type) == null)
			{
				// Value type (non-nullable): always append
				return Expression.Block(
						new[] { valueVariable },
						assignValue,
						appendExpression
				);
			}
			else
			{
				// Reference type or nullable value type: check for null
				return Expression.Block(
						new[] { valueVariable },
						assignValue,
						Expression.IfThen(
								Expression.NotEqual(valueVariable, Expression.Constant(null, valueVariable.Type)),
								appendExpression
						)
				);
			}
		}

		private static Expression BuildVariableAccessExpression(Expression instance, string path)
		{
			// Updated regex to support [^1] for from-end indexing
			var tokens = Regex.Matches(path, @"([a-zA-Z_][a-zA-Z0-9_]*|\[\^?\d+\])")
					.Cast<Match>()
					.Select(match => match.Value)
					.ToArray();

			Expression current = instance;
			foreach (var token in tokens)
			{
				if (token.StartsWith("[") && token.EndsWith("]"))
				{
					string indexToken = token.Substring(1, token.Length - 2);
					bool fromEnd = indexToken.StartsWith("^");
					int index = int.Parse(fromEnd ? indexToken.Substring(1) : indexToken);

					if (typeof(IEnumerable).IsAssignableFrom(current.Type) && current.Type != typeof(string))
					{
						var elementType = current.Type.IsArray
								? current.Type.GetElementType()!
								: current.Type.GetGenericArguments().FirstOrDefault() ?? typeof(object);

						var elementAtMethod = typeof(Enumerable).GetMethods()
								.First(method => method.Name == "ElementAt" && method.GetParameters().Length == 2)
								.MakeGenericMethod(elementType);

						if (fromEnd)
						{
							// Compute Count - index for from-end access
							var countMethod = typeof(Enumerable).GetMethods()
									.First(method => method.Name == "Count" && method.GetParameters().Length == 1)
									.MakeGenericMethod(elementType);

							var countCall = Expression.Call(countMethod, current);
							var fromEndIndex = Expression.Subtract(countCall, Expression.Constant(index));
							current = Expression.Call(elementAtMethod, current, fromEndIndex);
						}
						else
						{
							current = Expression.Call(elementAtMethod, current, Expression.Constant(index));
						}
					}
					else
					{
						throw new InvalidOperationException($"Type {current.Type} is not indexable.");
					}
				}
				else
				{
					var propertyInfo = current.Type.GetProperty(token, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
					if (propertyInfo != null)
					{
						current = Expression.Property(current, propertyInfo);
					}
					else
					{
						var fieldInfo = current.Type.GetField(token, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
						if (fieldInfo != null)
						{
							current = Expression.Field(current, fieldInfo);
						}
						else
						{
							throw new InvalidOperationException($"Property or field '{token}' not found on type {current.Type}.");
						}
					}
				}
			}
			return current;
		}
	}
}
