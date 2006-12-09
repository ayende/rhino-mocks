using System;
using System.Reflection;
using Castle.Core.Interceptor;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Utilities;
using Castle.DynamicProxy;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Records all the expectations for a mock
	/// </summary>
	public class RecordMockState : IMockState
	{
		#region Variables

		private MockRepository repository;
		private readonly IMockedObject mockedObject;
		private int methodCallsCount = 0;
		private IExpectation lastExpectation;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the last expectation.
		/// </summary>
		public IExpectation LastExpectation
		{
			get { return lastExpectation; }
			set { lastExpectation = value; }
		}

		/// <summary>
		/// Gets the total method calls count.
		/// </summary>
		public int MethodCallsCount
		{
			get { return methodCallsCount; }
		}

		/// <summary>
		/// Get the options for the last method call
		/// </summary>
		public IMethodOptions LastMethodOptions
		{
			get
			{
				if (lastExpectation == null)
					throw new InvalidOperationException("There is no matching last call on this object. Are you sure that the last call was a virtual or interface method call?");
				return new MethodOptions(repository, this, mockedObject, lastExpectation);
			}
		}

		/// <summary>
		/// Set the exception to throw when Verify is called.
		/// This is used to report exception that may have happened but where caught in the code.
		/// This way, they are reported anyway when Verify() is called.
		/// </summary>
		public void SetExceptionToThrowOnVerify(Exception ex)
		{
			//not implementing this, since there is never a call to Verify() anyway.
		}

		/// <summary>
		/// Gets the matching verify state for this state
		/// </summary>
		public IMockState VerifyState
		{
			get { throw InvalidOperationOnRecord(); }
		}

		#endregion

		#region C'tor

		/// <summary>
		/// Creates a new <see cref="RecordMockState"/> instance.
		/// </summary>
		/// <param name="repository">Repository.</param>
		/// <param name="mockedObject">The proxy that generates the method calls</param>
		public RecordMockState(IMockedObject mockedObject, MockRepository repository)
		{
			Validate.IsNotNull(mockedObject, "proxy");
			Validate.IsNotNull(repository, "repository");
			this.repository = repository;
			this.mockedObject = mockedObject;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add a method call for this state' mock.
		/// </summary>
        /// <param name="invocation">The invocation for this method</param>
		/// <param name="method">The method that was called</param>
		/// <param name="args">The arguments this method was called with</param>
		public object MethodCall(IInvocation invocation, MethodInfo method, params object[] args)
		{
			PreviousMethodIsClose();
			repository.lastMockedObject = mockedObject;
			MockRepository.lastRepository = repository;
			IExpectation expectation = new ArgsEqualExpectation(method, args);
			repository.Recorder.Record(mockedObject, method, expectation);
			lastExpectation = expectation;
			methodCallsCount++;
			return ReturnValueUtil.DefaultValue(method.ReturnType);
		}

		/// <summary>
		/// Verify that we can move to replay state and move 
		/// to the reply state.
		/// </summary>
		public virtual IMockState Replay()
		{
			PreviousMethodIsClose();
			return DoReplay();
		}

		/// <summary>
		/// Verify that we can move to replay state and move 
		/// to the reply state.
		/// </summary>
		protected virtual IMockState DoReplay()
		{
			return new ReplayMockState(this);
		}

		/// <summary>
		/// Verify that this mock expectations have passed.
		/// </summary>
		public virtual void Verify()
		{
			throw InvalidOperationOnRecord();
		}

        /// <summary>
        /// Gets a mock state that match the original mock state of the object.
        /// </summary>
        public virtual IMockState BackToRecord()
        {
            return new RecordMockState(mockedObject, repository);
        }

		#endregion

		#region Private Methods

		private void PreviousMethodIsClose()
		{
			if (lastExpectation != null && !lastExpectation.ActionsSatisfied)
				throw new InvalidOperationException("Previous method '" + lastExpectation.ErrorMessage + "' require a return value or an exception to throw.");
		}

		private Exception InvalidOperationOnRecord()
		{
			return new InvalidOperationException("This action is invalid when the mock object is in record state.");
        }
        #endregion

        #region Internal
        internal MockRepository Repository
        {
            get { return repository; }
        }

        internal IMockedObject Proxy
        {
            get { return mockedObject; }
        }

        #endregion
	}
}