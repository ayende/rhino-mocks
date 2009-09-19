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
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Records all the expectations for a mock and
	/// return a ReplayDynamicMockState when Replay()
	/// is called.
	/// </summary>
	public class RecordDynamicMockState : RecordMockState
	{
		/// <summary>
		/// Creates a new <see cref="RecordDynamicMockState"/> instance.
		/// </summary>
		/// <param name="repository">Repository.</param>
		/// <param name="mockedObject">The proxy that generates the method calls</param>
		public RecordDynamicMockState(IMockedObject mockedObject, MockRepository repository) : base(mockedObject, repository)
		{
		}

		/// <summary>
		/// Verify that we can move to replay state and move 
		/// to the reply state.
		/// </summary>
		protected override IMockState DoReplay()
		{
			return new ReplayDynamicMockState(this);
		}

        /// <summary>
        /// Get the default call count range expectation
        /// </summary>
        /// <returns></returns>
        protected override Range GetDefaultCallCountRangeExpectation()
        {
            return new Range(1, null);
        }

        /// <summary>
        /// Gets a mock state that match the original mock state of the object.
        /// </summary>
        public override IMockState BackToRecord()
        {
            return new RecordDynamicMockState(this.Proxy, Repository);
        }
	}
}
