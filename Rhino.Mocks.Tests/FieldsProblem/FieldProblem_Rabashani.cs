using Xunit;
using Rhino.Mocks.Tests.Model;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	
	public class FieldProblem_Rabashani
	{
		[Fact]
		public void CanMockInternalInterface()
		{
			MockRepository mocks = new MockRepository();
			IInternal mock = mocks.StrictMock<IInternal>();
			mock.Foo();
			mocks.ReplayAll();
			mock.Foo();
			mocks.VerifyAll();
		}

		[Fact]
		public void CanMockInternalClass()
		{
			MockRepository mocks = new MockRepository();
			Internal mock = mocks.StrictMock<Internal>();
			Expect.Call(mock.Bar()).Return("blah");
			mocks.ReplayAll();
			Assert.Equal("blah", mock.Bar());
			mocks.VerifyAll();
		}

		[Fact]
		public void CanPartialMockInternalClass()
		{
			MockRepository mocks = new MockRepository();
			Internal mock = mocks.PartialMock<Internal>();
			Expect.Call(mock.Foo()).Return("blah");
			mocks.ReplayAll();
			Assert.Equal("blah", mock.Foo());
			Assert.Equal("abc", mock.Bar());
			mocks.VerifyAll();
		}
	}
}
