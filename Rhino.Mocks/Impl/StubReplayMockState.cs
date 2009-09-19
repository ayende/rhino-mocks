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

using System.Reflection;
using Castle.Core.Interceptor;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Validate expectations on recorded methods, but in general completely ignoring them.
	/// Similar to <seealso cref="ReplayDynamicMockState"/> except that it would return a 
	/// <seealso cref="StubRecordMockState"/> when BackToRecord is called.
	/// </summary>
	internal class StubReplayMockState : ReplayMockState
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StubReplayMockState"/> class.
		/// </summary>
		/// <param name="previousState">The previous state for this method</param>
		public StubReplayMockState(RecordMockState previousState) : base(previousState)
		{
		}

		/// <summary>
		/// Add a method call for this state' mock.
		/// </summary>
        /// <param name="invocation">The invocation for this method</param>
		/// <param name="method">The method that was called</param>
		/// <param name="args">The arguments this method was called with</param>
		protected override object DoMethodCall(IInvocation invocation, MethodInfo method, params object[] args)
		{
			IExpectation expectation = repository.Replayer.GetRecordedExpectationOrNull(proxy, method, args);
			if (expectation != null)
			{
				RhinoMocks.Logger.LogReplayedExpectation(invocation, expectation);
				return expectation.ReturnOrThrow(invocation, args);
			}
			else
			{
				RhinoMocks.Logger.LogUnexpectedMethodCall(invocation, "Stub Mock: Unexpected method call ignored");
				return ReturnValueUtil.DefaultValue(method.ReturnType, invocation);
			}
		
		}

        /// <summary>
        /// Gets a mock state that matches the original mock state of the object.
        /// </summary>
        public override IMockState BackToRecord()
        {
            return new StubRecordMockState(proxy, repository);
        }

        public override void Verify()
        {
            //stub doesn't do verifications
        }

        public override IMockState VerifyState
        {
            get
            {
                return this;// there is no meaning to be verified in stubs.
            }
        }
	}
}
