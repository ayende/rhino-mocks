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
using Castle.DynamicProxy;
using Xunit;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests
{
	
	public class ExtendingRhinoMocks2
	{
		[Fact]
		public void DeleteThisTest()
		{
			MockRepository mockRepository = new MockRepository();
			MockedClass mock = mockRepository.StrictMock<MockedClass>();
			
			mock.Method("expectedParameter");

			mockRepository.ReplayAll();

			Assert.Throws<ExpectationViolationException>(() => mock.Method("invalidParameter"));
		}
	}

	public class ErnstMockRepository : MockRepository
	{
		public T StrictMockObjectThatVerifyAndCallOriginalMethod<T>()
		{
            return (T)CreateMockObject(typeof(T), new CreateMockState(CreateVerifyAndCallOriginalMockState), new Type[0]);
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
		/// AssertWasCalled that we can move to replay state and move 
		/// to the reply state.
		/// </summary>
		protected override IMockState DoReplay()
		{
			return new VerifyExpectationAndCallOriginalReplayState(this);
		}
	}

	internal class VerifyExpectationAndCallOriginalReplayState : ReplayMockState
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