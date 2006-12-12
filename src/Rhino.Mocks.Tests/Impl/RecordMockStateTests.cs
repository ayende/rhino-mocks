using System;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

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
			recordState.MethodCall(null, method, "");
			recordState.LastExpectation.ReturnValue = true;
			Assert.IsNotNull(Get.Recorder(mocks).GetAllExpectationsForProxyAndMethod(proxy, method), "Record state didn't record the method call.");
			recordState.MethodCall(null, method, "");
			recordState.LastExpectation.ReturnValue = true;
			Assert.AreEqual(2, recordState.MethodCallsCount);
		}

		[Test]
		public void MethodCallAddExpectation()
		{
			MockRepository mocks = new MockRepository();
			ProxyInstance proxy = new ProxyInstance(mocks);
			RecordMockState recordState = new RecordMockState(proxy, mocks);
            recordState.MethodCall(null, method, "1");
			recordState.LastExpectation.ReturnValue = false;
			Assert.AreEqual(1, Get.Recorder(mocks).GetAllExpectationsForProxyAndMethod(proxy, method).Count);
            recordState.MethodCall(null, method, "2");
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
            recordState.MethodCall(null, method, "");
			Assert.IsNotNull(recordState.LastExpectation);
			Assert.IsTrue(recordState.LastExpectation.IsExpected(new object[] {""}));
		}

		[Test]
		public void GetMethodOptionsForLastMethod()
		{
			MockRepository mocks = new MockRepository();
			RecordMockState recordState = new RecordMockState(new ProxyInstance(mocks), mocks);
            recordState.MethodCall(null, method, "");
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
		[ExpectedException(typeof (InvalidOperationException), "Previous method 'String.StartsWith(\"\");' require a return value or an exception to throw.")]
		public void CantMoveToReplayStateWithoutclosingLastMethod()
		{
			MockRepository mocks = new MockRepository();
			ProxyInstance proxy = new ProxyInstance(mocks);
			RecordMockState recordState = new RecordMockState(proxy, mocks);
            recordState.MethodCall(null, method, "");
			recordState.Replay();
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