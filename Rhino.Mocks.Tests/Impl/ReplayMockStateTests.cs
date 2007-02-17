using System;
using System.Reflection;
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Tests.Expectations;
using Rhino.Mocks.Tests.Utilities;

namespace Rhino.Mocks.Tests.Impl
{
	[TestFixture]
	public class ReplayMockStateTests
	{
		private static MockRepository mocks;
		private static MethodInfo startsWith;
		private static RecordMockState record;
		private ReplayMockState replay;
		private ProxyInstance proxy;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			proxy = new ProxyInstance(mocks);
			startsWith = CreateMethodInfo();
			record = new RecordMockState(proxy, ReplayMockStateTests.mocks);
            record.MethodCall(new FakeInvocation(startsWith), startsWith, "2");
			replay = new ReplayMockState(record);
		}


		[Test]
		public void CreatingReplayMockStateFromRecordMockStateCopiesTheExpectationList()
		{
			Assert.AreEqual(1, Get.Recorder(mocks).GetAllExpectationsForProxy(proxy).Count);
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "String.StartsWith(\"2\"); Expected #1, Actual #0.")]
		public void ExpectedMethodCallOnReplay()
		{
			ReplayMockState replay = new ReplayMockState(record);
			replay.Verify();
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "String.EndsWith(\"2\"); Expected #0, Actual #1.")]
		public void UnexpectedMethodCallOnReplayThrows()
		{
			MethodInfo endsWith = MethodCallTests.GetMethodInfo("EndsWith", "2");
            replay.MethodCall(null, endsWith, "2");
		}

		[Test]
		public void VerifyWhenAllExpectedCallsWereCalled()
		{
            this.replay.MethodCall(null, CreateMethodInfo(), "2");
			this.replay.Verify();
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "String.StartsWith(\"2\"); Expected #1, Actual #0.")]
		public void VerifyWhenNotAllExpectedCallsWereCalled()
		{
			ReplayMockState replay = new ReplayMockState(record);
			replay.Verify();
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "String.EndsWith(null); Expected #0, Actual #1.")]
		public void VerifyWhenMismatchArgsContainsNull()
		{
			MethodInfo endsWith = MethodCallTests.GetMethodInfo("EndsWith", "2");
            replay.MethodCall(null, endsWith, new object[1] { null });
		}

		[Test]
		public void VerifyReportsAllMissingExpectationsWhenCalled()
		{
			record.LastExpectation.ReturnValue = true;
			MethodInfo method = CreateMethodInfo();
			record.MethodCall(new FakeInvocation(method), method, "r");
			record.LastExpectation.ReturnValue = true;
			record.MethodCall(new FakeInvocation(method), method, "y");
			record.LastExpectation.ReturnValue = true;
			record.LastExpectation.Expected = new Range(2, 2);
			ReplayMockState replay = new ReplayMockState(record);
			try
			{
				replay.Verify();
			}
			catch (Exception e)
			{
				string message = "String.StartsWith(\"2\"); Expected #1, Actual #0.\r\n" +
					"String.StartsWith(\"r\"); Expected #1, Actual #0.\r\n" +
					"String.StartsWith(\"y\"); Expected #2, Actual #0.";
				Assert.AreEqual(message, e.Message);
			}
		}

		[Test]
		public void VerifyReportsAllMissingExpectationWhenCalledOnOrdered()
		{
			using (mocks.Ordered())
			{
				record.LastExpectation.ReturnValue = true;
				MethodInfo method = CreateMethodInfo();
                record.MethodCall(new FakeInvocation(method), method, "r");
				record.LastExpectation.ReturnValue = true;
                record.MethodCall(new FakeInvocation(method), method, "y");
				record.LastExpectation.ReturnValue = true;
				record.LastExpectation.Expected = new Range(2, 2);
			}
			ReplayMockState replay = new ReplayMockState(record);
			try
			{
				replay.Verify();
			}
			catch (Exception e)
			{
				string message = "String.StartsWith(\"2\"); Expected #1, Actual #0.\r\n" +
					"String.StartsWith(\"r\"); Expected #1, Actual #0.\r\n" +
					"String.StartsWith(\"y\"); Expected #2, Actual #0.";
				Assert.AreEqual(message, e.Message);
			}
		}

		#region Implementation

		private static MethodInfo CreateMethodInfo()
		{
			return MethodCallTests.GetMethodInfo("StartsWith", "");
		}

		#endregion
	}
}