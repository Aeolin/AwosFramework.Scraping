using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Reddit
{
	public sealed class AtomicInteger
	{
		private int _value;

		public AtomicInteger(int initialValue = 0)
		{
			_value = initialValue;
		}

		public static implicit operator int(AtomicInteger value) => value._value;
		public static implicit operator AtomicInteger(int value) => new AtomicInteger();

		public int Increment() => Interlocked.Increment(ref _value);
		public int Decrement() => Interlocked.Decrement(ref _value);
		public int Add(int value) => Interlocked.Add(ref _value, value);
		public int Get() => Interlocked.CompareExchange(ref _value, 0, 0);
		public void Set(int value) => Interlocked.Exchange(ref _value, value);
		public void Subtract(int value) => Interlocked.Add(ref _value, -value);
	}
}
