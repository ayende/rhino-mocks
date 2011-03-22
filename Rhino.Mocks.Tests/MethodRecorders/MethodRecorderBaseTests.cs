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
using Castle.DynamicProxy;
using Xunit;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.MethodRecorders;
using Rhino.Mocks.Generated;
using Rhino.Mocks.Tests.Expectations;

namespace Rhino.Mocks.Tests.MethodRecorders
{
	
	public class MethodRecorderBaseTests
	{
		private IMethodRecorder recorder;
		private TestMethodRecorder testRecorder;
		private object proxy;
		private MethodInfo method;
		private IExpectation expectation;
		private object[] args;

		public MethodRecorderBaseTests()
		{
			recorder = new UnorderedMethodRecorder(new ProxyMethodExpectationsDictionary());
			testRecorder = new TestMethodRecorder();
			recorder.AddRecorder(testRecorder);

			proxy = new object();
			method = typeof (object).GetMethod("ToString");
			expectation = new AnyArgsExpectation(new FakeInvocation(method), new Range(1, 1));
			args = new object[0];
		}

		[Fact]
		public void DoRecordCalled()
		{
			recorder.Record(proxy, method, expectation);
			Assert.True(testRecorder.DoRecordCalled);
		}

		[Fact]
		public void DoGetRecordedExpectationCalled()
		{
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
			typeof (MethodRecorderBase).
				GetField("replayerToCall", bindingFlags).
				SetValue(recorder, testRecorder);
			recorder.GetRecordedExpectation(new FakeInvocation(method), proxy, method, args);
			Assert.True(testRecorder.DoGetRecordedExpectationCalled);
		}

		[Fact]
		public void DoGetAllExpectationsForProxyAndMethodCalled()
		{
			recorder.GetAllExpectationsForProxyAndMethod(proxy, method);
			Assert.True(testRecorder.DoGetAllExpectationsForProxyAndMethodCalled);
		}

		[Fact]
		public void DoGetAllExpectationsForProxyCalled()
		{
			recorder.GetAllExpectationsForProxy(proxy);
			Assert.True(testRecorder.DoGetAllExpectationsForProxyCalled);
		}

		[Fact]
		public void DoReplaceExpectationCalled()
		{
			recorder.ReplaceExpectation(proxy, method, expectation, expectation);
			Assert.True(testRecorder.DoReplaceExpectationCalled);
		}

		[Fact]
		public void DoHasExpectationsCalled()
		{
			bool dummy = recorder.HasExpectations;
			Assert.True(testRecorder.DoHasExpectationsCalled);
		}

		[Fact]
		public void DoAddRecorderCalled()
		{
			recorder.AddRecorder(recorder);
			Assert.True(testRecorder.DoAddRecorderCalled);
		}

		[Fact]
		public void DoGetRecordedExpectationOrNullCalled()
		{
			recorder.GetRecordedExpectationOrNull(proxy, method, args);
			Assert.True(testRecorder.DoGetRecordedExpectationOrNullCalled);
		}

		[Fact]
		public void DoRemoveExpectationCalled()
		{
			recorder.RemoveExpectation(new AnyArgsExpectation(new FakeInvocation(method), new Range(1, 1)));
			Assert.True(testRecorder.DoRemoveExpectationCalled);
		}

		internal class TestMethodRecorder : MethodRecorderBase
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

			public TestMethodRecorder()
				: base(new ProxyMethodExpectationsDictionary())
			{
			}

			protected override void DoRecord(object proxy, MethodInfo method, IExpectation expectation)
			{
				DoRecordCalled = true;
			}

			protected override IExpectation DoGetRecordedExpectation(IInvocation invocation, object proxy, MethodInfo method, object[] args)
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
			public override ExpectationViolationException UnexpectedMethodCall(IInvocation invoication,object proxy, MethodInfo method, object[] args)
			{
				throw new NotImplementedException();
			}
		}
	}
}