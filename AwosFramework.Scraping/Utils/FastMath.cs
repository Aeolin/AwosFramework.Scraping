using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Utils
{
	public static class FastMath
	{
		public static int Min(params Span<int> values)
		{
			//Span<int> span = values;
			//if (Vector512.IsHardwareAccelerated)
			//{
			//	const int INT_COUNT = 512 / sizeof(int);
			//	int resultCount = span.Length;
			//	while(resultCount > 1)
			//	{
			//		resultCount = (int)Math.Ceiling(resultCount / (double)INT_COUNT);
			//		for(int i = 0; i < resultCount; i++)
			//		{
			//			var vec = Vector512.Create<int>(values, i * INT_COUNT);
			//			span[i] = Avx512F.MinScalar()
			//		}
			//	}

			//	return span[0];
			//}


			return values.ToArray().Min();

		}
	}
}
