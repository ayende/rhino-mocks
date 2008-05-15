using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	using Exceptions;

	[TestFixture]
	public class FieldProblem_Sam
	{
		[Test]
		public void Test()
		{
			MockRepository mocks = new MockRepository();
			SimpleOperations myMock = mocks.StrictMock<SimpleOperations>();
			Expect.Call(myMock.AddTwoValues(1, 2)).Return(3);
			mocks.ReplayAll();
			Assert.AreEqual(3, myMock.AddTwoValues(1, 2));
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "Unordered method call! The expected call is: 'Ordered: { IInterfaceWithThreeMethods.C(); }' but was: 'IInterfaceWithThreeMethods.B();'")]
		public void WillRememberExceptionInsideOrderRecorderEvenIfInsideCatchBlock()
		{
			MockRepository mockRepository = new MockRepository();
			IInterfaceWithThreeMethods interfaceWithThreeMethods = mockRepository.StrictMock<IInterfaceWithThreeMethods>();

			using (mockRepository.Ordered())
			{
				interfaceWithThreeMethods.A();
				interfaceWithThreeMethods.C();
			}

			mockRepository.ReplayAll();

			interfaceWithThreeMethods.A();
			try
			{
				interfaceWithThreeMethods.B();
			}
			catch { /* valid for code under test to catch all */ }
			interfaceWithThreeMethods.C();

			mockRepository.VerifyAll();
		}
	}

	public interface IInterfaceWithThreeMethods
	{
		void A();
		void B();
		void C();
	}

	public class SimpleOperations
	{
		public virtual int AddTwoValues(int x, int y)
		{
			return x + y;
		}
	}
}