using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Html.PostProcessing
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
	public class PostProcessorAttribute : Attribute
	{
		public PostProcessorAttribute(Type type)
		{
			if(type.IsAssignableTo(typeof(IPostProcessor)) == false)
				throw new ArgumentException("Type must implement IPostProcessor");

			PostProcessor = type;
		}

		public Type PostProcessor { get; init; }

	}

	public class PostProccesorAttribute<T> : PostProcessorAttribute where T : IPostProcessor
	{
		public PostProccesorAttribute() : base(typeof(T))
		{

		}
	}
}
