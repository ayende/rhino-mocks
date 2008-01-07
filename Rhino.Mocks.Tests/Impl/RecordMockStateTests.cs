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
using MbUnit.Framework;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Tests.Expectations;

namespace Rhino.Mocks.Tests.Impl
{
	[TestFixture]
	public class RecordMockStateTests
	{
        MethodInfo method = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });

		[Test]
		public void MethodCallOnRecordsAddToExpectations()
		{
			MockRepository mocks = new MockRepository();
			ProxyInstance proxy = new ProxyInstance(mocks);
			RecordMockState recordState = new RecordMockState(proxy, mocks);
			recordState.MethodCall(new FakeInvocation(method), method, "");
			recordState.LastExpectation.ReturnValue = true;
			Assert.IsNotNull(Get.Recorder(mocks).GetAllExpectationsForProxyAndMethod(proxy, method), "Record state didn't record the method call.");
			recordState.MethodCall(new FakeInvocation(method), method, "");
			recordState.LastExpectation.ReturnValue = true;
			Assert.AreEqual(2, recordState.MethodCallsCount);
		}

		[Test]
		public void MethodCallAddExpectation()
		{
			MockRepository mocks = new MockRepository();
			ProxyInstance proxy = new ProxyInstance(mocks);
			RecordMockState recordState = new RecordMockState(proxy, mocks);
            recordState.MethodCall(new FakeInvocation(method), method, "1");
			recordState.LastExpectation.ReturnValue = false;
			Assert.AreEqual(1, Get.Recorder(mocks).GetAllExpectationsForProxyAndMethod(proxy, method).Count);
            recordState.MethodCall(new FakeInvocation(method), method, "2");
			recordState.LastExpectation.ReturnValue = false;
			Assert.AreEqual(2, Get.Recorder(mocks).GetAllExpectationsForProxyAndMethod(proxy, method).Count);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "This action is invalid when the mock object is in record state.")]
		public void VerifyOnRecordThrows()
		{
			MockRepository mocks = new MockRepository();
			RecordMockState recordState = new RecordMockState(new ProxyInstance(mocks), mocks);
			recordState.Verify();
		}

		[Test]
		public void LastExpectationIsFilledOnCall()
		{
			MockRepository mocks = new MockRepository();
			RecordMockState recordState = new RecordMockState(new ProxyInstance(mocks), mocks);
			Assert.IsNull(recordState.LastExpectation);
            recordState.MethodCall(new FakeInvocation(method), method, "");
			Assert.IsNotNull(recordState.LastExpectation);
			Assert.IsTrue(recordState.LastExpectation.IsExpected(new object[] {""}));
		}

		[Test]
		public void GetMethodOptionsForLastMethod()
		{
			MockRepository mocks = new MockRepository();
			RecordMockState recordState = new RecordMockState(new ProxyInstance(mocks), mocks);
            recordState.MethodCall(new FakeInvocation(method), method, "");
			Assert.IsNotNull(recordState.LastMethodOptions);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: proxy")]
		public void PassingNullProxyCauseException()
		{
			new RecordMockState(null, null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: repository")]
		public void PassingNullmocksCauseException()
		{
			new RecordMockState(new ProxyInstance(null), null);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Previous method 'String.StartsWith(\"\");' requires a return value or an exception to throw.")]
		public void CantMoveToReplayStateWithoutclosingLastMethod()
		{
			MockRepository mocks = new MockRepository();
			ProxyInstance proxy = new ProxyInstance(mocks);
			RecordMockState recordState = new RecordMockState(proxy, mocks);
            recordState.MethodCall(new FakeInvocation(method), method, "");
			recordState.Replay();
		}

        [Test]
        public void ArgsEqualExpectationUsedForMethodsWithNoOutParameters()
        {
            MockRepository mocks = new MockRepository();
            RecordMockState recordState = new RecordMockState(new ProxyInstance(mocks), mocks);
            Assert.IsNull(recordState.LastExpectation);
            recordState.MethodCall(new FakeInvocation(method), method, "");
            Assert.IsInstanceOfType(typeof(ArgsEqualExpectation), recordState.LastExpectation);
        }

        [Test]
        public void ConstraintsExpectationUsedForMethodsWithNoOutParameters()
        {
            MethodInfo methodWithOutParams = typeof(Double).GetMethod("TryParse", new Type[] { typeof(string), typeof(Double).MakeByRefType() });
            MockRepository mocks = new MockRepository();
            RecordMockState recordState = new RecordMockState(new ProxyInstance(mocks), mocks);
            Assert.IsNull(recordState.LastExpectation);
            recordState.MethodCall(new FakeInvocation(methodWithOutParams), methodWithOutParams, "", 0);
            Assert.IsInstanceOfType(typeof(ConstraintsExpectation), recordState.LastExpectation);
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
