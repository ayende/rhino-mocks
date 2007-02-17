using MbUnit.Framework;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.Constraints
{
	[TestFixture]
	public class StringConstraintsTests
	{
		[Test]
		public void StartsWith()
		{
			Assert.IsTrue(Text.StartsWith("Hello").Eval("Hello World"));
			Assert.IsFalse(Text.StartsWith("Hello").Eval("World"));
			Assert.AreEqual("starts with \"Hello\"", Text.StartsWith("Hello").Message);
		}

		[Test]
		public void EndsWith()
		{
			Assert.IsTrue(Text.EndsWith("World").Eval("Hello World"));
			Assert.IsFalse(Text.EndsWith("Hello").Eval("World"));
			Assert.AreEqual("ends with \"Hello\"", Text.EndsWith("Hello").Message);
		}

		[Test]
		public void Contains()
		{
			AbstractConstraint c = Text.Contains("Ayende");
			Assert.IsTrue(c.Eval("Ayende Rahien"));
			Assert.IsFalse(c.Eval("Hello World"));
			Assert.AreEqual("contains \"Ayende\"", c.Message);
		}

		[Test]
		public void Like()
		{
			AbstractConstraint c = Text.Like("[Rr]ahien");
			Assert.IsTrue(c.Eval("Ayende Rahien"));
			Assert.IsFalse(c.Eval("Hello World"));
			Assert.AreEqual("like \"[Rr]ahien\"", c.Message);
		}

	}
}