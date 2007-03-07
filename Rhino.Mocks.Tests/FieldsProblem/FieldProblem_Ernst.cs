using System;
using MbUnit.Framework;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Ernst
	{
		[Test]
		public void CallOriginalMethodProblem2()
		{
			MockRepository mockRepository = new MockRepository();
			MockedClass mock = mockRepository.CreateMock<MockedClass>();

			mock.Method(null);
			LastCall.Constraints(Is.Equal("parameter")).CallOriginalMethod
				(OriginalCallOptions.CreateExpectation);

			mockRepository.ReplayAll();

			mock.Method("parameter");

			mockRepository.VerifyAll();
		}
	}

	public class MockedClass
	{
		public virtual void Method(string parameter)
		{
			if (parameter == null)
				throw new ArgumentNullException();

			//Something in this method must be executed
		}
	}
}