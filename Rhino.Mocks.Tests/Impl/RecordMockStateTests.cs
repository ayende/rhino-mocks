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
using Xunit;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Tests.Expectations;

namespace Rhino.Mocks.Tests.Impl
{
	
	public class RecordMockStateTests
	{
        MethodInfo method = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });

		[Fact]
		public void MethodCallOnRecordsAddToExpectations()
		{
			MockRepository mocks = new MockRepository();
			ProxyInstance proxy = new ProxyInstance(mocks);
			RecordMockState recordState = new RecordMockState(proxy, mocks);
			recordState.MethodCall(new FakeInvocation(method), method, "");
			recordState.LastExpectation.ReturnValue = true;
			Assert.NotNull(Get.Recorder(mocks).GetAllExpectationsForProxyAndMethod(proxy, method));
			recordState.MethodCall(new FakeInvocation(method), method, "");
			recordState.LastExpectation.ReturnValue = true;
			Assert.Equal(2, recordState.MethodCallsCount);
		}

		[Fact]
		public void MethodCallAddExpectation()
		{
			MockRepository mocks = new MockRepository();
			ProxyInstance proxy = new ProxyInstance(mocks);
			RecordMockState recordState = new RecordMockState(proxy, mocks);
            recordState.MethodCall(new FakeInvocation(method), method, "1");
			recordState.LastExpectation.ReturnValue = false;
			Assert.Equal(1, Get.Recorder(mocks).GetAllExpectationsForProxyAndMethod(proxy, method).Count);
            recordState.MethodCall(new FakeInvocation(method), method, "2");
			recordState.LastExpectation.ReturnValue = false;
			Assert.Equal(2, Get.Recorder(mocks).GetAllExpectationsForProxyAndMethod(proxy, method).Count);
		}

		[Fact]
		public void VerifyOnRecordThrows()
		{
			MockRepository mocks = new MockRepository();
			RecordMockState recordState = new RecordMockState(new ProxyInstance(mocks), mocks);
			Assert.Throws<InvalidOperationException>(
				"This action is invalid when the mock object is in record state.",
				() => recordState.Verify());
		}

        [Fact]
        public void VerifyOnRecordThrowsOneMockType() {
            MockRepository mocks = new MockRepository();
            RecordMockState recordState = new RecordMockState(new ProxyInstance(mocks, typeof(IAnimal)), mocks);
        	Assert.Throws<InvalidOperationException>(
        		"This action is invalid when the mock object {Rhino.Mocks.Tests.IAnimal} is in record state.",
        		() => recordState.Verify());
        }

        [Fact]
        public void VerifyOnRecordThrowsTwoMockTypes() {
            MockRepository mocks = new MockRepository();
            RecordMockState recordState = new RecordMockState(new ProxyInstance(mocks, typeof(IAnimal), typeof(IDemo)), mocks);
        	Assert.Throws<InvalidOperationException>(
        		"This action is invalid when the mock object {Rhino.Mocks.Tests.IAnimal, Rhino.Mocks.Tests.IDemo} is in record state.",
        		() => recordState.Verify());
        }

        [Fact]
		public void LastExpectationIsFilledOnCall()
		{
			MockRepository mocks = new MockRepository();
			RecordMockState recordState = new RecordMockState(new ProxyInstance(mocks), mocks);
			Assert.Null(recordState.LastExpectation);
            recordState.MethodCall(new FakeInvocation(method), method, "");
			Assert.NotNull(recordState.LastExpectation);
			Assert.True(recordState.LastExpectation.IsExpected(new object[] {""}));
		}

		[Fact]
		public void GetMethodOptionsForLastMethod()
		{
			MockRepository mocks = new MockRepository();
			RecordMockState recordState = new RecordMockState(new ProxyInstance(mocks), mocks);
            recordState.MethodCall(new FakeInvocation(method), method, "");
			Assert.NotNull(recordState.LastMethodOptions);
		}

		[Fact]
		public void PassingNullProxyCauseException()
		{
			Assert.Throws<ArgumentNullException>(
				"Value cannot be null.\r\nParameter name: proxy",
				() => new RecordMockState(null, null));
		}

		[Fact]
		public void PassingNullmocksCauseException()
		{
			Assert.Throws<ArgumentNullException>(
				"Value cannot be null.\r\nParameter name: repository",
				() => new RecordMockState(new ProxyInstance(null), null));
		}

		[Fact]
		public void CantMoveToReplayStateWithoutclosingLastMethod()
		{
			MockRepository mocks = new MockRepository();
			ProxyInstance proxy = new ProxyInstance(mocks);
			RecordMockState recordState = new RecordMockState(proxy, mocks);
            recordState.MethodCall(new FakeInvocation(method), method, "");
			Assert.Throws<InvalidOperationException>(
				"Previous method 'String.StartsWith(\"\");' requires a return value or an exception to throw.",
				() => recordState.Replay());
		}

        [Fact]
        public void ArgsEqualExpectationUsedForMethodsWithNoOutParameters()
        {
            MockRepository mocks = new MockRepository();
            RecordMockState recordState = new RecordMockState(new ProxyInstance(mocks), mocks);
            Assert.Null(recordState.LastExpectation);
            recordState.MethodCall(new FakeInvocation(method), method, "");
            Assert.IsType(typeof(ArgsEqualExpectation), recordState.LastExpectation);
        }

        [Fact]
        public void ConstraintsExpectationUsedForMethodsWithNoOutParameters()
        {
            MethodInfo methodWithOutParams = typeof(Double).GetMethod("TryParse", new Type[] { typeof(string), typeof(Double).MakeByRefType() });
            MockRepository mocks = new MockRepository();
            RecordMockState recordState = new RecordMockState(new ProxyInstance(mocks), mocks);
            Assert.Null(recordState.LastExpectation);
            recordState.MethodCall(new FakeInvocation(methodWithOutParams), methodWithOutParams, "", 0);
			Assert.IsType(typeof(ConstraintsExpectation), recordState.LastExpectation);
        }
	}

	
	public class Get
	{
		private static PropertyInfo prop = typeof (MockRepository).GetProperty("Recorder", BindingFlags.Instance | BindingFlags.NonPublic);

		public static IMethodRecorder Recorder(MockRepository mocks)
		{
			return (IMethodRecorder) prop.GetValue(mocks, null);
		}
	}

}
