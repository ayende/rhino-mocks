using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	
	public class FieldProblem_Chad
	{
		[Fact]
		public void SetupResult_For_writeable_property_on_stub_should_be_ignored()
		{
			MockRepository mocks = new MockRepository();
			TestClass test = mocks.Stub<TestClass>();
			SetupResult.For(test.ReadOnly).Return("foo");

			const string expected =
				@"You are trying to set an expectation on a property that was defined to use PropertyBehavior.
Instead of writing code such as this: mockObject.Stub(x => x.SomeProperty).Return(42);
You can use the property directly to achieve the same result: mockObject.SomeProperty = 42;";

			Assert.Throws<InvalidOperationException>(expected, () => SetupResult.For(test.ReadWrite).PropertyBehavior());
		}
		public class TestClass
		{
			public virtual string ReadOnly { get { return ""; } }
			public virtual string ReadWrite { get { return null; } set { } }
		}
	}
}