using System;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.MethodRecorders;
using Rhino.Mocks.Generated;

namespace Rhino.Mocks.Tests.MethodRecorders
{
	[TestFixture]
	public class MethodRecorderBaseTests
	{
		private IMethodRecorder recorder;
		private TestMethodRecorder testRecorder;
		private object proxy;
		private MethodInfo method;
		private IExpectation expectation;
		private object[] args;

		[SetUp]
		public void SetUp()
		{
			recorder = new UnorderedMethodRecorder();
			testRecorder = new TestMethodRecorder();
			recorder.AddRecorder(testRecorder);

			proxy = new object();
			method = typeof (object).GetMethod("ToString");
			expectation = new AnyArgsExpectation(method);
			args = new object[0];
		}

		[Test]
		public void DoRecordCalled()
		{
			recorder.Record(proxy, method, expectation);
			Assert.IsTrue(testRecorder.DoRecordCalled);
		}

		[Test]
		public void DoGetRecordedExpectationCalled()
		{
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
			typeof (MethodRecorderBase).
				GetField("replayerToCall", bindingFlags).
				SetValue(recorder, testRecorder);
			recorder.GetRecordedExpectation(proxy, method, args);
			Assert.IsTrue(testRecorder.DoGetRecordedExpectationCalled);
		}

		[Test]
		public void DoGetAllExpectationsForProxyAndMethodCalled()
		{
			recorder.GetAllExpectationsForProxyAndMethod(proxy, method);
			Assert.IsTrue(testRecorder.DoGetAllExpectationsForProxyAndMethodCalled);
		}

		[Test]
		public void DoGetAllExpectationsForProxyCalled()
		{
			recorder.GetAllExpectationsForProxy(proxy);
			Assert.IsTrue(testRecorder.DoGetAllExpectationsForProxyCalled);
		}

		[Test]
		public void DoReplaceExpectationCalled()
		{
			recorder.ReplaceExpectation(proxy, method, expectation, expectation);
			Assert.IsTrue(testRecorder.DoReplaceExpectationCalled);
		}

		[Test]
		public void DoHasExpectationsCalled()
		{
			bool dummy = recorder.HasExpectations;
			Assert.IsTrue(testRecorder.DoHasExpectationsCalled);
		}

		[Test]
		public void DoAddRecorderCalled()
		{
			recorder.AddRecorder(recorder);
			Assert.IsTrue(testRecorder.DoAddRecorderCalled);
		}

		[Test]
		public void DoGetRecordedExpectationOrNullCalled()
		{
			recorder.GetRecordedExpectationOrNull(proxy, method, args);
			Assert.IsTrue(testRecorder.DoGetRecordedExpectationOrNullCalled);
		}

		[Test]
		public void DoRemoveExpectationCalled()
		{
			recorder.RemoveExpectation(new AnyArgsExpectation(method));
			Assert.IsTrue(testRecorder.DoRemoveExpectationCalled);
		}

		public class TestMethodRecorder : MethodRecorderBase
		{
			public bool DoRecordCalled,
				DoGetRecordedExpectationCalled,
				DoGetAllExpectationsForProxyAndMethodCalled,
				DoGetAllExpectationsForProxyCalled,
				DoReplaceExpectationCalled,
				DoHasExpectationsCalled,
				DoAddRecorderCalled,
				DoGetRecordedExpectationOrNullCalled,
				DoExpectedCallsCalled,
				DoRemoveExpectationCalled;


			protected override void DoRecord(object proxy, MethodInfo method, IExpectation expectation)
			{
				DoRecordCalled = true;
			}

			protected override IExpectation DoGetRecordedExpectation(object proxy, MethodInfo method, object[] args)
			{
				DoGetRecordedExpectationCalled = true;
				return null;
			}

			public override ExpectationsList GetAllExpectationsForProxyAndMethod(object proxy, MethodInfo method)
			{
				DoGetAllExpectationsForProxyAndMethodCalled = true;
				return new ExpectationsList();
			}

			protected override ExpectationsList DoGetAllExpectationsForProxy(object proxy)
			{
				DoGetAllExpectationsForProxyCalled = true;
				return new ExpectationsList();
			}

			protected override void DoReplaceExpectation(object proxy, MethodInfo method, IExpectation oldExpectation, IExpectation newExpectation)
			{
				DoReplaceExpectationCalled = true;
			}

			protected override bool DoHasExpectations
			{
				get
				{
					DoHasExpectationsCalled = true;
					return false;
				}
			}

			protected override void DoRemoveExpectation(IExpectation expectation)
			{
				DoRemoveExpectationCalled = true;
			}

			protected override IExpectation DoGetRecordedExpectationOrNull(object proxy, MethodInfo method, object[] args)
			{
				DoGetRecordedExpectationOrNullCalled = true;
				return null;
			}

			protected override void DoAddRecorder(IMethodRecorder recorder)
			{
				DoAddRecorderCalled = true;
			}

			public override string GetExpectedCallsMessage()
			{
				DoExpectedCallsCalled = true;
				return null;
			}

			/// <summary>
			/// Get the expectation for this method on this object with this arguments 
			/// </summary>
			public override ExpectationViolationException UnexpectedMethodCall(object proxy, MethodInfo method, object[] args)
			{
				throw new NotImplementedException();
			}
		}
	}
}