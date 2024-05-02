using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Core
{
	public class BinaryContent
	{
		public Stream Content { get; init; }
		public string ContentType { get; init; }

		public static BinaryContent OfStream(Stream stream, string contentType)
		{
			var mem = new MemoryStream();
			stream.CopyTo(mem);
			mem.Position=0;
			return new BinaryContent(mem, contentType);
		}

		public BinaryContent(Stream content, string contentType)
		{
			Content=content;
			ContentType=contentType;
		}
	}
}
