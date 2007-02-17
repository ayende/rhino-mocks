using MbUnit.Framework;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.Constraints
{
	[TestFixture]
	public class ListConstraintTests
	{
		[Test]
		public void InIs()
		{
			AbstractConstraint list = List.IsIn('a');
			Assert.IsTrue(list.Eval("ayende"));
			Assert.IsFalse(list.Eval("sheep"));
			Assert.AreEqual("list contains [a]", list.Message);
		}

		[Test]
		public void OneOf()
		{
			AbstractConstraint list = List.OneOf(new string[] {"Ayende", "Rahien", "Hello", "World"});
			Assert.IsTrue(list.Eval("Ayende"));
			Assert.IsFalse(list.Eval("sheep"));
			Assert.AreEqual("one of [Ayende, Rahien, Hello, World]", list.Message);
		}

		[Test]
		public void Equal()
		{
			AbstractConstraint list = List.Equal(new string[] {"Ayende", "Rahien", "Hello", "World"});
			Assert.IsTrue(list.Eval(new string[] {"Ayende", "Rahien", "Hello", "World"}));
			Assert.IsFalse(list.Eval(new string[] {"Ayende", "Rahien", "World", "Hello"}));
			Assert.IsFalse(list.Eval(new string[] {"Ayende", "Rahien", "World"}));
			Assert.IsFalse(list.Eval(5));
			Assert.AreEqual("equal to collection [Ayende, Rahien, Hello, World]", list.Message);

		}
	}
}