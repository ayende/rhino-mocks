using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Sam
	{
		[Test]
		public void Test()
		{
			MockRepository mocks = new MockRepository();
			SimpleOperations myMock = mocks.CreateMock<SimpleOperations>();
			Expect.Call(myMock.AddTwoValues(1, 2)).Return(3);
			mocks.ReplayAll();
			Assert.AreEqual(3, myMock.AddTwoValues(1, 2));
			mocks.VerifyAll();
		}
	}

	public class SimpleOperations
	{
		public virtual int AddTwoValues(int x, int y)
		{
			return x + y;
		}
	}
}