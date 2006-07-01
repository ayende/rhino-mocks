using System;
using System.Reflection;
using System.Text;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;
using Castle.DynamicProxy;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Validate all expectations on a mock
	/// </summary>
	public class ReplayMockState : IMockState
	{
		#region Variables

		/// <summary>
		/// The repository for this state
		/// </summary>
		protected MockRepository repository;
		/// <summary>
		/// The proxy object for this state
		/// </summary>
		protected IMockedObject proxy;

		#endregion

		#region Properties

		/// <summary>
		/// Get the options for the last method call
		/// </summary>
		IMethodOptions IMockState.LastMethodOptions
		{
			get
			{
				throw InvalidInReplayState();
			}
		}

		/// <summary>
		/// Gets the matching verify state for this state
		/// </summary>
		public IMockState VerifyState
		{
			get { return new VerifiedMockState(this); }
		}

		#endregion

		#region C'tor

		/// <summary>
		/// Creates a new <see cref="ReplayMockState"/> instance.
		/// </summary>
		/// <param name="previousState">The previous state for this method</param>
		public ReplayMockState(RecordMockState previousState)
		{
			this.repository = previousState.Repository;
			this.proxy = previousState.Proxy;
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
			IExpectation expectation = repository.Replayer.GetRepeatableExpectation(proxy, method, args);
			if (expectation != null)
				return expectation.ReturnOrThrow(args);
            return DoMethodCall(invocation, method, args);
		}

		/// <summary>
		/// Add a method call for this state' mock.
		/// This allows derived method to cleanly get a the setupresult behavior while adding
		/// their own.
		/// </summary>
        /// <param name="invocation">The invocation for this method</param>
		/// <param name="method">The method that was called</param>
		/// <param name="args">The arguments this method was called with</param>
		protected virtual object DoMethodCall(IInvocation invocation, MethodInfo method, object[] args)
		{
			IExpectation expectation = repository.Replayer.GetRecordedExpectation(proxy, method, args);
			return expectation.ReturnOrThrow(args);
		}

		/// <summary>
		/// Verify that this mock expectations have passed.
		/// </summary>
		public void Verify()
		{
			StringBuilder sb = new StringBuilder();
			bool verifiedFailed = false;
			foreach (IExpectation expectation in repository.Recorder.GetAllExpectationsForProxy(proxy))
			{
				if (expectation.ExpectationSatisfied == false)
				{
					if (verifiedFailed)
						sb.Append("\r\n");
					sb.Append(BuildVerificationFailureMessage(expectation));
					verifiedFailed = true;
				}
			}
			if (verifiedFailed)
				throw new ExpectationViolationException(sb.ToString());
		}

		/// <summary>
		/// Verify that we can move to replay state and move 
		/// to the reply state.
		/// </summary>
		public virtual IMockState Replay()
		{
			throw InvalidInReplayState();
		}

        /// <summary>
        /// Gets a mock state that match the original mock state of the object.
        /// </summary>
        public virtual IMockState BackToRecord()
        {
            return new RecordMockState(proxy, repository);
        }

		#endregion

		#region Private Methods

		private Exception InvalidInReplayState()
		{
			return new InvalidOperationException("This action is invalid when the mock object is in replay state.");
		}

		private StringBuilder BuildVerificationFailureMessage(IExpectation expectation)
		{
			StringBuilder sb = new StringBuilder();
			string expectationMessege = expectation.ErrorMessage;
			sb.Append(expectationMessege).Append(' ');
			sb.Append("Expected #");
			sb.Append(expectation.Expected.ToString()).Append(", ");
			sb.Append("Actual #").Append(expectation.ActualCalls).Append('.');
			return sb;
		}

		#endregion
	}
}