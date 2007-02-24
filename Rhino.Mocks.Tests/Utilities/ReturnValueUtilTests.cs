using MbUnit.Framework;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Tests.Utilities
{
	[TestFixture]
	public class ReturnValueUtilTests
	{
		[Test]
		public void DefaultReturnValue()
		{
			Assert.IsNull(ReturnValueUtil.DefaultValue(typeof (string),null));
			Assert.AreEqual(0, ReturnValueUtil.DefaultValue(typeof (int),null));
			Assert.AreEqual((short) 0, ReturnValueUtil.DefaultValue(typeof (short),null));
			Assert.AreEqual((char) 0, ReturnValueUtil.DefaultValue(typeof (char),null));
			Assert.AreEqual(0L, ReturnValueUtil.DefaultValue(typeof (long),null));
			Assert.AreEqual(0f, ReturnValueUtil.DefaultValue(typeof (float),null));
			Assert.AreEqual(0d, ReturnValueUtil.DefaultValue(typeof (double),null));
			Assert.AreEqual(TestEnum.DefaultValue, ReturnValueUtil.DefaultValue(typeof (TestEnum),null));
		}

		private enum TestEnum
		{
			DefaultValue,
			NonDefaultValue
		}
	}
}