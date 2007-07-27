using System.Reflection;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
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

		[Test]
		public void StubGenericInterface_CanReadWriteProperties()
		{
			MockRepository mocks = new MockRepository();
			ITestGen<int> test = mocks.Stub<ITestGen<int>>();

			mocks.ReplayAll();

			test.Foo = 10;
			Assert.AreEqual(10, test.Foo);

			mocks.VerifyAll();
		}

		[Test]
		public void StubInterface_CanReadWriteProperties()
		{
			MockRepository mocks = new MockRepository();
			ITestNormal test = mocks.Stub<ITestNormal>();

			mocks.ReplayAll();

			test.Foo = 10;
			Assert.AreEqual(10, test.Foo);

			mocks.VerifyAll();
		}

		[Test]
		public void MockGenericInterface_CanSetProperties()
		{
			MockRepository mocks = new MockRepository();
			ITestGen<int> test = mocks.CreateMock<ITestGen<int>>();

			SetupResult.For(test.Foo).PropertyBehavior();

			mocks.ReplayAll();

			test.Foo = 10;
			Assert.AreEqual(10, test.Foo);

			mocks.VerifyAll();
		}

		[Test]
		public void MockNormalInterface_CanSetProperties()
		{
			MockRepository mocks = new MockRepository();
			ITestNormal test = mocks.CreateMock<ITestNormal>();

			SetupResult.For(test.Foo).PropertyBehavior();

			mocks.ReplayAll();

			test.Foo = 10;
			Assert.AreEqual(10, test.Foo);

			mocks.VerifyAll();
		}
	}
}
