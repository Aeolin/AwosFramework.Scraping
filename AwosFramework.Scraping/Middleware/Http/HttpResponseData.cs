using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware.Http
{
	public struct HttpResponseData
	{
		public Stream Stream { get; init; } 
		public string MimeType { get; init; }

		public HttpResponseData(Stream stream, string mimeType)
		{
			Stream = stream;
			MimeType = mimeType;
		}
	}
}
