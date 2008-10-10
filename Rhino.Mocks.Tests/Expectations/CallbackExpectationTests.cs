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
using Rhino.Mocks.Tests.Callbacks;

namespace Rhino.Mocks.Tests.Expectations
{
	[TestFixture]
	public class CallbackExpectationTests :AbstractExpectationTests
	{
		private MockRepository mocks;
		private IDemo demo;
		private CallbackExpectation callback;
		private MethodInfo method;
		private bool callbackCalled;

		protected override IExpectation GetExpectation(MethodInfo m, Range r, int actual)
		{
			CallbackExpectation expectation = new CallbackExpectation(new FakeInvocation(m), new DelegateDefinations.NoArgsDelegate(VoidNoArgs), new Range(1, 1));
			SetupExpectation(expectation, r, actual);
			return expectation;
		}

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			demo = (IDemo) mocks.StrictMock(typeof (IDemo));
			method = typeof (IDemo).GetMethod("VoidThreeArgs");
			callbackCalled = false;
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),"Callback arguments didn't match the method arguments")]
		public void ExceptionWhenArgsDontMatch()
		{
			callback = new CallbackExpectation(new FakeInvocation(method), new DelegateDefinations.NoArgsDelegate(VoidNoArgs), new Range(1, 1));
		}

		[Test]
		public void CallMethodWhenTestIsExpected()
		{
			callback = new CallbackExpectation(new FakeInvocation(method), new DelegateDefinations.ThreeArgsDelegate(ThreeArgsDelegateMethod), new Range(1, 1));
			callback.IsExpected(new object[] {1, "", 3.3f});
			Assert.IsTrue(callbackCalled);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Callbacks must return a boolean")]
		public void CallbackDoesntReturnBool()
		{
			callback = new CallbackExpectation(new FakeInvocation(method), new DelegateDefinations.VoidThreeArgsDelegate(VoidThreeArgsDelegateMethod), new Range(1, 1));
			callback.IsExpected(new object[] {1, "", 3.3f});
			Assert.IsTrue(callbackCalled);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Callback arguments didn't match the method arguments")]
		public void CallbackWithDifferentSignature_NumArgsDifferent()
		{
			callback = new CallbackExpectation(new FakeInvocation(method), new DelegateDefinations.StringDelegate("".StartsWith), new Range(1, 1));
			callback.IsExpected(new object[] {1, "", 3.3f});
			Assert.IsTrue(callbackCalled);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Callback arguments didn't match the method arguments")]
		public void CallBackWithDifferentSignature()
		{
			callback = new CallbackExpectation(new FakeInvocation(method), new DelegateDefinations.IntArgDelegate(OneArg), new Range(1, 1));
		}

		#region Implementation

		private bool VoidNoArgs()
		{
			return true;
		}

		private bool OneArg(int i)
		{
			return true;
		}

		private void VoidThreeArgsDelegateMethod(int i, string s, float f)
		{
		}

		private bool ThreeArgsDelegateMethod(int i, string s, float f)
		{
			callbackCalled = true;
			return true;
		}

		#endregion
	}
}