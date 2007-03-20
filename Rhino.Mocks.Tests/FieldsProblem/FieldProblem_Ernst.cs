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

		[Test]
		public void CanUseBackToRecordOnMethodsThatCallToCallOriginalMethod()
		{
			MockRepository repository = new MockRepository();
			TestClass mock = repository.CreateMock<TestClass>();

			mock.Method();
			LastCall.CallOriginalMethod
				(OriginalCallOptions.NoExpectation);

			repository.ReplayAll();
			mock.Method();
			repository.VerifyAll();

			repository.BackToRecordAll();

			mock.Method();
			LastCall.Throw(new ApplicationException());

			repository.ReplayAll();

			try
			{
				mock.Method();
				Assert.Fail();
			}
			catch (ApplicationException ex)
			{
			}
			repository.VerifyAll();
		}


		[Test]
		public void CanUseBackToRecordOnMethodsThatCallPropertyBehavior()
		{
			MockRepository repository = new MockRepository();
			TestClass mock = repository.CreateMock<TestClass>();

			Expect.Call(mock.Id).PropertyBehavior();

			repository.ReplayAll();
			mock.Id = 4;
			int d = mock.Id;
			Assert.AreEqual(4,d );
			repository.VerifyAll();

			repository.BackToRecordAll();

			Expect.Call(mock.Id).Return(5);

			repository.ReplayAll();

			Assert.AreEqual(5, mock.Id);

			repository.VerifyAll();
		}
	}

	public class TestClass
	{
		private int id;


		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual void Method()
		{
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