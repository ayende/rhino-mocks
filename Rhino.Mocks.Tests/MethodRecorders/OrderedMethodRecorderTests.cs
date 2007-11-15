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


using MbUnit.Framework;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Generated;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.MethodRecorders;
using Rhino.Mocks.Tests.Expectations;

namespace Rhino.Mocks.Tests.MethodRecorders
{
	[TestFixture]
	public class OrderedMethodRecorderTests : UnorderedMethodRecorderTests
	{
		[Test]
		public void RecordMethodsAndReplayThemInSameOrder()
		{
			OrderedMethodRecorder recorder = new OrderedMethodRecorder(new ProxyMethodExpectationsDictionary());
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);
			recorder.Record(this.demo, voidThreeArgs, new AnyArgsExpectation(new FakeInvocation(voidThreeArgs)));

			Assert.IsNotNull(recorder.GetRecordedExpectation(new FakeInvocation(voidNoArgs), this.demo, this.voidNoArgs, new object[0]));
			Assert.IsNotNull(recorder.GetRecordedExpectation(new FakeInvocation(voidNoArgs), this.demo, this.voidThreeArgs, new object[0]));
		}

		[Test]
		public void GetAllExpectationsForProxyWithNestedOrdering()
		{
			recorder.AddRecorder(this.CreateRecorder());
			recorder.Record(this.demo, this.voidNoArgs, expectationTwo);
			recorder.Record(this.demo, this.voidThreeArgs, expectationOne);
			//move to replayer
			recorder.GetRecordedExpectation(new FakeInvocation(voidNoArgs), demo, voidNoArgs, new object[0]);
			ExpectationsList expectations = recorder.GetAllExpectationsForProxy(demo);
			Assert.AreEqual(1, expectations.Count);
		}

		[Test]
		public void RemoveExpectationWhenNestedOrdering()
		{
			IExpectation newExpectation = new ArgsEqualExpectation(new FakeInvocation(voidThreeArgs), new object[]{1,null,1f});
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);
			recorder.AddRecorder(CreateRecorder());
			recorder.Record(this.demo, this.voidThreeArgs, expectationTwo);
			recorder.Record(this.demo, this.voidNoArgs, newExpectation);
			recorder.RemoveExpectation(expectationTwo);

			//move to replayer, but also remove one expectation from consideration
			recorder.GetRecordedExpectation(new FakeInvocation(voidNoArgs), demo, voidNoArgs, new object[0]);
			
			ExpectationsList expectations = recorder.GetAllExpectationsForProxy(demo);
			Assert.AreEqual(1, expectations.Count);
			Assert.AreEqual(expectations[0],newExpectation);
		}

		[Test]
		public void GetAllExpectationForProxyAndMethodWithNestedOrdering()
		{
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);
			recorder.AddRecorder(CreateRecorder());
			recorder.Record(this.demo, this.voidThreeArgs, expectationOne);
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);
			//move to replayer
			recorder.GetRecordedExpectation(new FakeInvocation(voidNoArgs), demo, voidNoArgs, new object[0]);
			
			ExpectationsList expectations = recorder.GetAllExpectationsForProxyAndMethod(demo, voidNoArgs);
			Assert.AreEqual(1, expectations.Count);
			expectations = recorder.GetAllExpectationsForProxyAndMethod(demo, voidThreeArgs);
			Assert.AreEqual(1, expectations.Count);
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "Unordered method call! The expected call is: 'Ordered: { IDemo.VoidNoArgs(); }' but was: 'IDemo.VoidNoArgs();'")]
		public void RecordMethodsAndReplayThemOutOfOrder()
		{
			OrderedMethodRecorder recorder = new OrderedMethodRecorder(new ProxyMethodExpectationsDictionary());
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);
			recorder.Record(this.demo, this.voidThreeArgs, expectationOne);

			Assert.IsNotNull(recorder.GetRecordedExpectation(new FakeInvocation(this.voidNoArgs), this.demo, this.voidNoArgs, new object[0]));
			recorder.GetRecordedExpectation(new FakeInvocation(this.voidNoArgs), this.demo, this.voidNoArgs, new object[0]);
		}

		
		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "Unordered method call! The expected call is: 'Ordered: { Message: Test Message\nIDemo.VoidNoArgs(); }' but was: 'IDemo.VoidNoArgs();'")]
		public void RecordMethodsAndReplayThemOutOfOrder_WillUseMessage()
		{
			OrderedMethodRecorder recorder = new OrderedMethodRecorder(new ProxyMethodExpectationsDictionary());
			expectationOne.Message = "Test Message";
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);
			recorder.Record(this.demo, this.voidThreeArgs, expectationOne);

			Assert.IsNotNull(recorder.GetRecordedExpectation(new FakeInvocation(this.voidNoArgs), this.demo, this.voidNoArgs, new object[0]));
			recorder.GetRecordedExpectation(new FakeInvocation(this.voidNoArgs), this.demo, this.voidNoArgs, new object[0]);
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "Unordered method call! The expected call is: 'Ordered: { No method call is expected }' but was: 'IDemo.VoidNoArgs();'")]
		public void ReplayWhenNoMethodIsExpected()
		{
			OrderedMethodRecorder recorder = new OrderedMethodRecorder(new ProxyMethodExpectationsDictionary());
			recorder.GetRecordedExpectation(new FakeInvocation(this.voidNoArgs), this.demo, this.voidNoArgs, new object[0]);
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IDemo.VoidNoArgs(); Expected #0, Actual #1.")]
		public void ReplayErrorWhenInOtherReplayer()
		{
			OrderedMethodRecorder recorder = new OrderedMethodRecorder(new ProxyMethodExpectationsDictionary());
			recorder.AddRecorder(new UnorderedMethodRecorder(new ProxyMethodExpectationsDictionary()));
			recorder.GetRecordedExpectation(new FakeInvocation(this.voidNoArgs), this.demo, this.voidNoArgs, new object[0]);
		}

		protected override IMethodRecorder CreateRecorder()
		{
			return new OrderedMethodRecorder(new ProxyMethodExpectationsDictionary());
		}
	}
}