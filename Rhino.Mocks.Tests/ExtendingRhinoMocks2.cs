using System;
using System.Reflection;
using Castle.Core.Interceptor;
using NUnit.Framework;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class ExtendingRhinoMocks2
	{
		[Test]
		[ExpectedException(typeof (ExpectationViolationException))]
		public void DeleteThisTest()
		{
			MockRepository mockRepository = new MockRepository();
			MockedClass mock = mockRepository.CreateMock<MockedClass>();

			mock.Method("expectedParameter");

			mockRepository.ReplayAll();

			mock.Method("invalidParameter");

			mockRepository.VerifyAll();
		}
	}

	public class ErnstMockRepository : MockRepository
	{
		public T CreateMockObjectThatVerifyAndCallOriginalMethod<T>()
		{
			return (T) CreateMockObject(typeof (T), new CreateMockState(CreateVerifyAndCallOriginalMockState), new Type[0]);
		}

		private IMockState CreateVerifyAndCallOriginalMockState(IMockedObject mockedObject)
		{
			return new VerifyExpectationAndCallOriginalRecordState(mockedObject, this);
		}
	}

	public class MockedClass
	{
		public virtual void Method(string parameter)
		{
			//Something in this method must be executed
		}
	}

	public class VerifyExpectationAndCallOriginalRecordState : RecordMockState
	{
		public VerifyExpectationAndCallOriginalRecordState(IMockedObject mockedObject, MockRepository repository) : base(mockedObject, repository)
		{
		}


		/// <summary>
		/// Verify that we can move to replay state and move 
		/// to the reply state.
		/// </summary>
		protected override IMockState DoReplay()
		{
			return new VerifyExpectationAndCallOriginalReplayState(this);
		}
	}

	public class VerifyExpectationAndCallOriginalReplayState : ReplayMockState
	{
		public VerifyExpectationAndCallOriginalReplayState(RecordMockState previousState)
			: base(previousState)
		{
		}


		protected override object DoMethodCall(IInvocation invocation, MethodInfo method, object[] args)
		{
			object result = base.DoMethodCall(invocation, method, args);
			invocation.Proceed();
			return result;
		}
	}
}