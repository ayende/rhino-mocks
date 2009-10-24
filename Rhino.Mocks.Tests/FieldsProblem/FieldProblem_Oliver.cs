using System.Reflection;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	
	public class FieldProblem_Oliver
	{
		public interface ITestGen<T>
		{
			int Foo { get; set; }
		}

		public interface ITestNormal
		{
			int Foo { get; set;}
		}

		[Fact]
		public void StubGenericInterface_CanReadWriteProperties()
		{
			MockRepository mocks = new MockRepository();
			ITestGen<int> test = mocks.Stub<ITestGen<int>>();

			mocks.ReplayAll();

			test.Foo = 10;
			Assert.Equal(10, test.Foo);

			mocks.VerifyAll();
		}

		[Fact]
		public void StubInterface_CanReadWriteProperties()
		{
			MockRepository mocks = new MockRepository();
			ITestNormal test = mocks.Stub<ITestNormal>();

			mocks.ReplayAll();

			test.Foo = 10;
			Assert.Equal(10, test.Foo);

			mocks.VerifyAll();
		}

		[Fact]
		public void MockGenericInterface_CanSetProperties()
		{
			MockRepository mocks = new MockRepository();
			ITestGen<int> test = mocks.StrictMock<ITestGen<int>>();

			SetupResult.For(test.Foo).PropertyBehavior();

			mocks.ReplayAll();

			test.Foo = 10;
			Assert.Equal(10, test.Foo);

			mocks.VerifyAll();
		}

		[Fact]
		public void MockNormalInterface_CanSetProperties()
		{
			MockRepository mocks = new MockRepository();
			ITestNormal test = mocks.StrictMock<ITestNormal>();

			SetupResult.For(test.Foo).PropertyBehavior();

			mocks.ReplayAll();

			test.Foo = 10;
			Assert.Equal(10, test.Foo);

			mocks.VerifyAll();
		}
	}
}
