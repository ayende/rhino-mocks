using NUnit.Framework;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Tests.Utilities
{
	[TestFixture]
	public class ReturnValueUtilTests
	{
		[Test]
		public void DefaultReturnValue()
		{
			Assert.IsNull(ReturnValueUtil.DefaultValue(typeof (string)));
			Assert.AreEqual(0, ReturnValueUtil.DefaultValue(typeof (int)));
			Assert.AreEqual((short) 0, ReturnValueUtil.DefaultValue(typeof (short)));
			Assert.AreEqual((char) 0, ReturnValueUtil.DefaultValue(typeof (char)));
			Assert.AreEqual(0L, ReturnValueUtil.DefaultValue(typeof (long)));
			Assert.AreEqual(0f, ReturnValueUtil.DefaultValue(typeof (float)));
			Assert.AreEqual(0d, ReturnValueUtil.DefaultValue(typeof (double)));
			Assert.AreEqual(TestEnum.DefaultValue, ReturnValueUtil.DefaultValue(typeof (TestEnum)));
		}

		private enum TestEnum
		{
			DefaultValue,
			NonDefaultValue
		}
	}
}