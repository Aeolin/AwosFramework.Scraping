﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Routing
{
	[AttributeUsage(AttributeTargets.Method)]
	public class RouteAttribute : Attribute
	{
		public string Path { get; init; }
		public string Host { get; init; }

		public RouteAttribute(string path)
		{
			if (path.Contains("://"))
			{
				var hostOffset = path.IndexOf("://") + 3;
				var firstSegment = path.IndexOf("/", hostOffset) + 1;
				Host = path.Substring(0, firstSegment);
				Path = path.Substring(firstSegment);
			}
			else
			{
				Path = path;
			}
		}
	}
}