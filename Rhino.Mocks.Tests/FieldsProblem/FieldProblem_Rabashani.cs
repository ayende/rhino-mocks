using MbUnit.Framework;
using Rhino.Mocks.Tests.Model;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Rabashani
	{
		[Test]
		public void CanMockInternalInterface()
		{
			MockRepository mocks = new MockRepository();
			var mock = mocks.StrictMock<IInternal>();
			mock.Foo();
			mocks.ReplayAll();
			mock.Foo();
			mocks.VerifyAll();
		}

		[Test]
		public void CanMockInternalClass()
		{
			MockRepository mocks = new MockRepository();
			var mock = mocks.StrictMock<Internal>();
			Expect.Call(mock.Bar()).Return("blah");
			mocks.ReplayAll();
			Assert.AreEqual("blah", mock.Bar());
			mocks.VerifyAll();
		}

		[Test]
		public void CanPartialMockInternalClass()
		{
			MockRepository mocks = new MockRepository();
			var mock = mocks.PartialMock<Internal>();
			Expect.Call(mock.Foo()).Return("blah");
			mocks.ReplayAll();
			Assert.AreEqual("blah", mock.Foo());
			Assert.AreEqual("abc", mock.Bar());
			mocks.VerifyAll();
		}
	}
}