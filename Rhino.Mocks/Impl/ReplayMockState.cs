#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;

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

		private Exception exceptionToThrowOnVerify;

		#endregion

		#region Properties

		/// <summary>
		/// Get the options for the last method call
		/// </summary>
		public IMethodOptions<T> GetLastMethodOptions<T>()
		{
			throw InvalidInReplayState();
		}

		/// <summary>
		/// Get the options for the last method call
		/// </summary>
		public IMethodOptions<object> LastMethodOptions
		{
			get { throw InvalidInReplayState(); }
		}

		/// <summary>
		/// Gets the matching verify state for this state
		/// </summary>
		public virtual IMockState VerifyState
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
			{
				RhinoMocks.Logger.LogReplayedExpectation(invocation, expectation);
				return expectation.ReturnOrThrow(invocation,args);
			}
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
			IExpectation expectation = repository.Replayer.GetRecordedExpectation(invocation, proxy, method, args);
			RhinoMocks.Logger.LogReplayedExpectation(invocation, expectation);
			return expectation.ReturnOrThrow(invocation,args);
		}

		/// <summary>
		/// Set the exception to throw when Verify is called.
		/// This is used to report exception that may have happened but where caught in the code.
		/// This way, they are reported anyway when Verify() is called.
		/// </summary>
		public void SetExceptionToThrowOnVerify(Exception ex)
		{
			this.exceptionToThrowOnVerify = ex;
		}

        /// <summary>
        /// not relevant
        /// </summary>
	    public void NotifyCallOnPropertyBehavior()
	    {
	        // doesn't deal with recording anyway
	    }

	    /// <summary>
		/// Verify that this mock expectations have passed.
		/// </summary>
		public virtual void Verify()
		{
			if (exceptionToThrowOnVerify != null)
				throw exceptionToThrowOnVerify;
			StringBuilder sb = new StringBuilder();
			bool verifiedFailed = false;
			foreach (IExpectation expectation in repository.Recorder.GetAllExpectationsForProxy(proxy))
			{
				if (expectation.ExpectationSatisfied == false)
				{
					if (verifiedFailed)
						sb.Append("\r\n");
					sb.Append(expectation.BuildVerificationFailureMessage());
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

		#endregion
	}
}