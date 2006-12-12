using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.MethodRecorders;
using Rhino.Mocks.Tests.Impl;

namespace Rhino.Mocks.Tests.MethodRecorders
{
	[TestFixture]
	public class RecorderChangerTests
	{
		private object proxy;
		private MethodInfo method;
		private IExpectation expectation;
		private object[] args;
		private MockRepository mocks;

		[SetUp]
		public void SetUp()
		{
			proxy = new object();
			method = typeof (object).GetMethod("ToString");
			expectation = new AnyArgsExpectation(method);
			args = new object[0];
			mocks = new MockRepository();
		}

		[Test]
		public void ChangeRecorderOnCtor()
		{
			IMethodRecorder recorder = new UnorderedMethodRecorder();
			MethodRecorderBaseTests.TestMethodRecorder testRecorder = new MethodRecorderBaseTests.TestMethodRecorder();
			new RecorderChanger(mocks, recorder, testRecorder);
			recorder.GetAllExpectationsForProxy(new object());
			Assert.IsTrue(testRecorder.DoGetAllExpectationsForProxyCalled);
			Assert.AreSame(testRecorder, Get.Recorder(mocks));
		}

		[Test]
		public void ChangeBackOnDispose()
		{
			IMethodRecorder recorder = new UnorderedMethodRecorder();
			MethodRecorderBaseTests.TestMethodRecorder testRecorder = new MethodRecorderBaseTests.TestMethodRecorder();
			using (new RecorderChanger(mocks, recorder, testRecorder))
			{
				Assert.AreSame(testRecorder, Get.Recorder(mocks));
			}
			Assert.AreNotSame(testRecorder, Get.Recorder(mocks));
			testRecorder.DoRecordCalled = false;
			recorder.Record(proxy, method, expectation);
			Assert.IsFalse(testRecorder.DoRecordCalled);

		}

		[Test]
		public void ChangeRecorderTwice()
		{
			IMethodRecorder recorder = new UnorderedMethodRecorder();
			MethodRecorderBaseTests.TestMethodRecorder testRecorder = new MethodRecorderBaseTests.TestMethodRecorder();
			using (new RecorderChanger(mocks, recorder, testRecorder))
			{
				Assert.AreSame(testRecorder, Get.Recorder(mocks));
				MethodRecorderBaseTests.TestMethodRecorder testRecorder2 = new MethodRecorderBaseTests.TestMethodRecorder();
				using (new RecorderChanger(mocks, recorder, testRecorder2))
				{
					Assert.AreSame(testRecorder2, Get.Recorder(mocks));
					testRecorder2.DoRecordCalled = false;
					recorder.Record(proxy, method, expectation);
					Assert.IsTrue(testRecorder2.DoRecordCalled);
				}
				Assert.AreSame(testRecorder, Get.Recorder(mocks));
				testRecorder.DoRecordCalled = false;
				recorder.Record(proxy, method, expectation);
				Assert.IsTrue(testRecorder.DoRecordCalled);
			}
			Assert.AreNotSame(testRecorder, Get.Recorder(mocks));
			testRecorder.DoRecordCalled = false;
			recorder.Record(proxy, method, expectation);
			Assert.IsFalse(testRecorder.DoRecordCalled);
		}
	}
}