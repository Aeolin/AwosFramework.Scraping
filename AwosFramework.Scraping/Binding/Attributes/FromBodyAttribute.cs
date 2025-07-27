using AwosFramework.Scraping.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Binding.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class FromBodyAttribute : Attribute
	{
		public DeserializationType DeserializationType { get; set; } 

		public FromBodyAttribute(DeserializationType deserializationType = DeserializationType.Json)
		{
			DeserializationType=deserializationType;
		}
	}
}
