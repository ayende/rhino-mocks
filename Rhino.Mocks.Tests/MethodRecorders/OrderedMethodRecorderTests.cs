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
			OrderedMethodRecorder recorder = new OrderedMethodRecorder();
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
			OrderedMethodRecorder recorder = new OrderedMethodRecorder();
			recorder.Record(this.demo, this.voidNoArgs, expectationOne);
			recorder.Record(this.demo, this.voidThreeArgs, expectationOne);

			Assert.IsNotNull(recorder.GetRecordedExpectation(new FakeInvocation(this.voidNoArgs), this.demo, this.voidNoArgs, new object[0]));
			recorder.GetRecordedExpectation(new FakeInvocation(this.voidNoArgs), this.demo, this.voidNoArgs, new object[0]);
		}

		
		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "Unordered method call! The expected call is: 'Ordered: { Message: Test Message\nIDemo.VoidNoArgs(); }' but was: 'IDemo.VoidNoArgs();'")]
		public void RecordMethodsAndReplayThemOutOfOrder_WillUseMessage()
		{
			OrderedMethodRecorder recorder = new OrderedMethodRecorder();
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
			OrderedMethodRecorder recorder = new OrderedMethodRecorder();
			recorder.GetRecordedExpectation(new FakeInvocation(this.voidNoArgs), this.demo, this.voidNoArgs, new object[0]);
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "Unordered method call! The expected call is: 'Ordered: { Unordered: {  } }' but was: 'IDemo.VoidNoArgs();'")]
		public void ReplayErrorWhenInOtherReplayer()
		{
			OrderedMethodRecorder recorder = new OrderedMethodRecorder();
			recorder.AddRecorder(new UnorderedMethodRecorder());
			recorder.GetRecordedExpectation(new FakeInvocation(this.voidNoArgs), this.demo, this.voidNoArgs, new object[0]);
		}

		protected override IMethodRecorder CreateRecorder()
		{
			return new OrderedMethodRecorder();
		}
	}
}