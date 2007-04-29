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
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Generated;
using Rhino.Mocks.Tests.Expectations;

namespace Rhino.Mocks.Tests.MethodRecorders
{
	public abstract class IMethodRecorderTests
	{
		private MockRepository mocks;
		protected IDemo demo;
		protected MethodInfo voidNoArgs;
		protected AnyArgsExpectation expectationOne,expectationTwo;
		protected IMethodRecorder recorder;
		protected MethodInfo voidThreeArgs;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			demo = this.mocks.CreateMock(typeof (IDemo)) as IDemo;
			voidNoArgs = typeof (IDemo).GetMethod("VoidNoArgs");
			voidThreeArgs = typeof (IDemo).GetMethod("VoidThreeStringArgs");
			expectationOne = new AnyArgsExpectation(new FakeInvocation(this.voidNoArgs));
			expectationTwo = new AnyArgsExpectation(new FakeInvocation(voidThreeArgs));
			recorder = CreateRecorder();
			ChildSetup();
		}

		[Test]
		public void HasExpectationsStartsEmpty()
		{
			Assert.IsFalse(recorder.HasExpectations);
		}

		[Test]
		public void HasExpectationsAfterAddingExpectation()
		{
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);
			Assert.IsTrue(recorder.HasExpectations);
		}

		[Test]
		public void HasExpectationsAfterGettingRecordedExpectation()
		{
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);
			recorder.GetRecordedExpectation(new FakeInvocation(voidNoArgs),demo, voidNoArgs, new object[0]);
			Assert.IsFalse(recorder.HasExpectations);
		}

		[Test]
		public void GetAllExpectationForProxyAndMethod()
		{
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);
			recorder.Record(this.demo, this.voidThreeArgs, expectationOne);
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);

			ExpectationsList expectations = recorder.GetAllExpectationsForProxyAndMethod(demo, voidNoArgs);
			Assert.AreEqual(2, expectations.Count);
			expectations = recorder.GetAllExpectationsForProxyAndMethod(demo, voidThreeArgs);
			Assert.AreEqual(1, expectations.Count);
		}

		[Test]
		public void GetAllExpectationsForProxy()
		{
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);
			recorder.Record(this.demo, this.voidThreeArgs, expectationOne);
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);
			ExpectationsList expectations = recorder.GetAllExpectationsForProxy(demo);
			Assert.AreEqual(3, expectations.Count);

		}


		[Test]
		public void ReplaceExpectation()
		{
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);
			AnyArgsExpectation newExpectation = new AnyArgsExpectation(new FakeInvocation(voidNoArgs));
			recorder.ReplaceExpectation(demo, voidNoArgs, expectationOne, newExpectation);
			ExpectationsList list = recorder.GetAllExpectationsForProxyAndMethod(demo, voidNoArgs);
			Assert.AreSame(newExpectation, list[0]);
		}

		[Test]
		public void ReplaceExpectationWhenNestingOrdering()
		{
			recorder.AddRecorder(CreateRecorder());
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);
			
			AnyArgsExpectation newExpectation = new AnyArgsExpectation(new FakeInvocation(voidNoArgs));
			recorder.ReplaceExpectation(demo, voidNoArgs, expectationOne, newExpectation);
			ExpectationsList list = recorder.GetAllExpectationsForProxyAndMethod(demo, voidNoArgs);
			Assert.AreSame(newExpectation, list[0]);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: proxy")]
		public void RecordProxyNullThrows()
		{
			this.recorder.Record(null, voidNoArgs, expectationOne);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: method")]
		public void RecordMethodNullThrows()
		{
			recorder.Record(demo, null, expectationOne);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: expectation")]
		public void RecordArgsNullThrows()
		{
			recorder.Record(demo, voidNoArgs, null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: proxy")]
		public void WasRecordedProxyNullThrows()
		{
			recorder.Record(demo, voidNoArgs, expectationOne);
			recorder.GetRecordedExpectation(new FakeInvocation(voidNoArgs), null, voidNoArgs, new object[0]);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: method")]
		public void WasRecordedMethodNullThrows()
		{
			recorder.Record(demo, voidNoArgs, expectationOne);
			recorder.GetRecordedExpectation(new FakeInvocation(null), demo, null, new object[0]);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: args")]
		public void WasRecordedArgsNullThrows()
		{
			recorder.Record(demo, voidNoArgs, expectationOne);
			recorder.GetRecordedExpectation(new FakeInvocation(voidNoArgs), demo, voidNoArgs, null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: method")]
		public void GetAllExpectationsMethodNullThrows()
		{
			recorder.GetAllExpectationsForProxyAndMethod(demo, null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: proxy")]
		public void GetAllExpectationsProxyNullThrows()
		{
			recorder.GetAllExpectationsForProxyAndMethod(null, voidNoArgs);

		}

		protected abstract IMethodRecorder CreateRecorder();

		protected virtual void ChildSetup()
		{
		}
	}
}