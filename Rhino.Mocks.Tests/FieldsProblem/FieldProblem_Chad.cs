using System;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Chad
	{
		[Test]
		[ExpectedException(typeof(InvalidOperationException),
			@"You are trying to set an expectation on a property that was defined to use PropertyBehavior.
Instead of writing code such as this: mockObject.Stub(x => x.SomeProperty).Return(42);
You can use the property directly to achieve the same result: mockObject.SomeProperty = 42;")]
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