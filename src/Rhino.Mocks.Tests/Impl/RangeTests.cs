using NUnit.Framework;
using Rhino.Mocks.Impl;

namespace Rhino.Mocks.Tests.Impl
{
	[TestFixture]
	public class RangeTests
	{
		[Test]
		public void RangePropetiesReturnTheSameValuesAsThosePassedInCtor()
		{
			Range range = new Range(30, 50);
			Assert.AreEqual(30, range.Min);
			Assert.AreEqual(50, range.Max);
		}

		[Test]
		public void RangeToString()
		{
			Range range = new Range(30, 50);
			Assert.AreEqual("30..50", range.ToString());
		}

		[Test]
		public void RangeToStringWhenMinMaxEqual()
		{
			Range range = new Range(30, 30);
			Assert.AreEqual("30", range.ToString());
		}

		[Test]
		public void RangeToStringWhenMinZeroAndMaxNonZero()
		{
			Range range = new Range(0, 30);
			Assert.AreEqual("30", range.ToString());
		}
	}
}