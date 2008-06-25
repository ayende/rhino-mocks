using System;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Chad
	{
		[Test]
		[ExpectedException(typeof(InvalidOperationException),
			"The result for TestClass.get_ReadOnly(); has already been setup. Properties are already stubbed with PropertyBehavior by default, no action is required")]
		public void SetupResult_For_writeable_property_on_stub_should_be_ignored()
		{
			MockRepository mocks = new MockRepository();
			TestClass test = mocks.Stub<TestClass>();
			SetupResult.For(test.ReadOnly).Return("foo");
			SetupResult.For(test.ReadWrite).PropertyBehavior();
		}
		public class TestClass
		{
			public virtual string ReadOnly { get { return ""; } }
			public virtual string ReadWrite { get { return null; } set { } }
		}
	}
}