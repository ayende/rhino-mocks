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