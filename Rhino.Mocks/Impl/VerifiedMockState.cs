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
using Castle.Core.Interceptor;
using Rhino.Mocks.Interfaces;
using Castle.DynamicProxy;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Throw an object already verified when accessed
	/// </summary>
	public class VerifiedMockState : IMockState
	{
        IMockState previous;

        /// <summary>
        /// Create a new instance of VerifiedMockState 
        /// </summary>
        /// <param name="previous">The previous mock state, used to get the initial record state</param>
        public VerifiedMockState(IMockState previous)
        {
            this.previous = previous;
        }

		/// <summary>
		/// Gets the matching verify state for this state
		/// </summary>
		public IMockState VerifyState
		{
			get { throw InvalidInVerifiedState(); }
		}

		/// <summary>
		/// Add a method call for this state' mock.
		/// </summary>
        /// <param name="invocation">The invocation for this method</param>
		/// <param name="method">The method that was called</param>
		/// <param name="args">The arguments this method was called with</param>
        public object MethodCall(IInvocation invocation, MethodInfo method, params object[] args)
		{
			if(method.Name == "Finalize") // skip finalizers
				return null;
			throw InvalidInVerifiedState();
		}

		/// <summary>
		/// Verify that this mock expectations have passed.
		/// </summary>
		public void Verify()
		{
			throw InvalidInVerifiedState();
		}

		/// <summary>
		/// Verify that we can move to replay state and move 
		/// to the reply state.
		/// </summary>
		public IMockState Replay()
		{
			throw InvalidInVerifiedState();
		}

        /// <summary>
        /// Gets a mock state that match the original mock state of the object.
        /// </summary>
        public virtual IMockState BackToRecord()
        {
            return previous.BackToRecord();
        }

		/// <summary>
		/// Get the options for the last method call
		/// </summary>
		public IMethodOptions<T> GetLastMethodOptions<T>()
		{
			throw InvalidInVerifiedState(); 
		}

		/// <summary>
		/// Get the options for the last method call
		/// </summary>
		public IMethodOptions<object> LastMethodOptions
		{
			get { throw InvalidInVerifiedState(); }
		}

		/// <summary>
		/// Set the exception to throw when Verify is called.
		/// This is used to report exception that may have happened but where caught in the code.
		/// This way, they are reported anyway when Verify() is called.
		/// </summary>
		public void SetExceptionToThrowOnVerify(Exception ex)
		{
			//not implementing this, we are already verified.
        }

        /// <summary>
        /// not relevant
        /// </summary>
        public void NotifyCallOnPropertyBehavior()
	    {
	        // doesn't deal with recording anyway
	    }

	    private Exception InvalidInVerifiedState()
		{
			return new InvalidOperationException("This action is invalid when the mock object is in verified state.");
		}
	}
}
