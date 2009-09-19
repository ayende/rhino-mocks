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
using System.Collections;
using MbUnit.Core.Exceptions;
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Tests.Callbacks;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class RhinoMockTests
	{
		private MockRepository mocks;
		private IDemo demo;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			demo = this.mocks.StrictMock(typeof (IDemo)) as IDemo;
		}

		[Test]
		public void CallsAreNotOrderDependant()
		{
			this.demo.ReturnStringNoArgs();
			LastCall.On(this.demo).Return(null);
			this.demo.VoidStringArg("Hello");
			this.mocks.Replay(this.demo);
			this.demo.VoidStringArg("Hello");
			this.demo.ReturnStringNoArgs();
			this.mocks.Verify(this.demo);
		}

		[Test]
		public void OrderedCallsTrackingAsExpected()
		{
			RecordOrdered(mocks, demo);

			mocks.Replay(demo);
			demo.ReturnStringNoArgs();
			demo.VoidNoArgs();
			demo.VoidNoArgs();
			demo.VoidStringArg("Hello");
			demo.VoidStringArg("World");
			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "Unordered method call! The expected call is: 'Ordered: { IDemo.VoidNoArgs(); }' but was: 'IDemo.VoidStringArg(\"Hello\");'")]
		public void GetDocumentationMessageWhenExpectationNotMet()
		{
			RecordOrdered(mocks, demo);
			mocks.Replay(demo);

			demo.ReturnStringNoArgs();
			demo.VoidNoArgs();
			demo.VoidStringArg("Hello");

			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "Message: Called to prefar foo for bar\nIDemo.VoidNoArgs(); Expected #1, Actual #0.")]
		public void WillDisplayDocumentationMessageIfNotCalled()
		{
			demo.VoidNoArgs();
			LastCall.On(demo)
				.IgnoreArguments()
				.Message("Called to prefar foo for bar");
			
			mocks.Replay(demo);

			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), @"IDemo.VoidNoArgs(); Expected #1, Actual #2.
Message: Should be called only once")]
		public void WillDiplayDocumentationMessageIfCalledTooMuch()
		{
			demo.VoidNoArgs();
			LastCall.Message("Should be called only once");
			
			mocks.ReplayAll();

			demo.VoidNoArgs();
			demo.VoidNoArgs();
			
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Invalid call, the last call has been used or no call has been made (make sure that you are calling a virtual (C#) / Overridable (VB) method).")]
		public void LastMockedObjectIsNullAfterDisposingMockRepository()
		{
		    MockRepository mocks = new MockRepository();
				mocks.ReplayAll();				
		    mocks.VerifyAll();
			LastCall.IgnoreArguments();
		}

		[Test]
		public void MixOrderedAndUnorderedBehaviour()
		{
			using (mocks.Ordered())
			{
				demo.EnumNoArgs();
				LastCall.On(demo).Return(EnumDemo.Dozen).Repeat.Twice();
				demo.VoidStringArg("Ayende");
				using (mocks.Unordered())
				{
					demo.VoidStringArg("Rahien");
					demo.VoidThreeStringArgs("1", "2", "3");
				}
				demo.StringArgString("Hello");
				LastCall.On(demo).Return("World");
			}
			mocks.Replay(demo);
			Assert.AreEqual(EnumDemo.Dozen, demo.EnumNoArgs());
			Assert.AreEqual(EnumDemo.Dozen, demo.EnumNoArgs());
			demo.VoidStringArg("Ayende");
			demo.VoidThreeStringArgs("1", "2", "3");
			demo.VoidStringArg("Rahien");
			Assert.AreEqual("World", demo.StringArgString("Hello"));

			mocks.Verify(demo);
		}

		[Test]
		public void ChangingRecordersWhenReplayingDoesNotInterruptVerification()
		{
			demo.VoidStringArg("ayende");
			mocks.Replay(demo);
			using (mocks.Ordered())
			{
				demo.VoidStringArg("ayende");
			}
			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Can't start replaying because Ordered or Unordered properties were call and not yet disposed.")]
		public void CallingReplayInOrderringThrows()
		{
			demo.VoidStringArg("ayende");
			using (mocks.Ordered())
			{
				mocks.Replay(demo);
			}
		}

		[Test]
		public void UsingSeveralObjectAndMixingOrderAndUnorder()
		{
			IList second = mocks.StrictMock(typeof (IList)) as IList;
			using (mocks.Ordered())
			{
				demo.EnumNoArgs();
				LastCall.On(demo).Return(EnumDemo.Dozen).Repeat.Twice();
				second.Clear();
				demo.VoidStringArg("Ayende");
				using (mocks.Unordered())
				{
					int i = second.Count;
					LastCall.On(second).Repeat.Twice().Return(3);
					demo.VoidStringArg("Rahien");
					demo.VoidThreeStringArgs("1", "2", "3");
				}
				demo.StringArgString("Hello");
				LastCall.On(demo).Return("World");
				second.IndexOf(null);
				LastCall.On(second).Return(2);
			}

			mocks.Replay(demo);
			mocks.Replay(second);

			Assert.AreEqual(EnumDemo.Dozen, demo.EnumNoArgs());
			Assert.AreEqual(EnumDemo.Dozen, demo.EnumNoArgs());
			second.Clear();
			demo.VoidStringArg("Ayende");
			Assert.AreEqual(3, second.Count);
			demo.VoidThreeStringArgs("1", "2", "3");
			Assert.AreEqual(3, second.Count);
			demo.VoidStringArg("Rahien");
			Assert.AreEqual("World", demo.StringArgString("Hello"));
			second.IndexOf(null);
			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "Unordered method call! The expected call is: 'Ordered: { IDemo.EnumNoArgs(); }' but was: 'IList.Clear();'")]
		public void SeveralMocksUsingOrdered()
		{
			IList second = mocks.StrictMock(typeof (IList)) as IList;
			using (mocks.Ordered())
			{
				demo.EnumNoArgs();
				LastCall.On(demo).Return(EnumDemo.Dozen).Repeat.Twice();
				second.Clear();
				demo.VoidStringArg("Ayende");
				using (mocks.Unordered())
				{
					int i = second.Count;
					LastCall.On(second).Repeat.Twice().Return(3);
					demo.VoidStringArg("Rahien");
					demo.VoidThreeStringArgs("1", "2", "3");
				}
				demo.StringArgString("Hello");
				LastCall.On(demo).Return("World");
				second.IndexOf(null);
				LastCall.On(second).Return(2);
			}

			mocks.Replay(demo);
			mocks.Replay(second);

			demo.EnumNoArgs();
			second.Clear();
		}

		[Test]
		public void RecursiveExpectationsOnUnordered()
		{
			demo = (IDemo) mocks.StrictMock(typeof (IDemo));
			demo.VoidNoArgs();
			LastCall.On(demo).Callback(new DelegateDefinations.NoArgsDelegate(CallMethodOnDemo));
			demo.VoidStringArg("Ayende");
			mocks.Replay(demo);
			demo.VoidNoArgs();
			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "Unordered method call! The expected call is: 'Ordered: { IDemo.VoidNoArgs(callback method: RhinoMockTests.CallMethodOnDemo); }' but was: 'IDemo.VoidStringArg(\"Ayende\");'")]
		public void RecursiveExpectationsOnOrdered()
		{
			demo = (IDemo) mocks.StrictMock(typeof (IDemo));
			using (mocks.Ordered())
			{
				demo.VoidNoArgs();
				LastCall.On(demo).Callback(CallMethodOnDemo);
				demo.VoidStringArg("Ayende");
			}
			mocks.Replay(demo);
			demo.VoidNoArgs();
		}


		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "IDemo.VoidThreeStringArgs(\"c\", \"b\", \"a\"); Expected #0, Actual #1.\r\nIDemo.VoidThreeStringArgs(\"a\", \"b\", \"c\"); Expected #1, Actual #0.")]
		public void GetArgsOfEpectedAndActualMethodCallOnException()
		{
			demo = (IDemo) mocks.StrictMock(typeof (IDemo));
			demo.VoidThreeStringArgs("a","b","c");
			mocks.Replay(demo);
			demo.VoidThreeStringArgs("c","b","a");
		}


		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "Unordered method call! The expected call is: 'Ordered: { IDemo.VoidStringArg(\"Ayende\"); }' but was: 'IDemo.VoidThreeStringArgs(\"\", \"\", \"\");'")]
		public void SteppingFromInnerOrderringToOuterWithoutFullifingAllOrderringInInnerThrows()
		{
			demo = (IDemo) mocks.StrictMock(typeof (IDemo));
			demo.VoidThreeStringArgs("", "", "");
			using (mocks.Ordered())
			{
				demo.VoidNoArgs();
				demo.VoidStringArg("Ayende");
			}
			mocks.Replay(demo);
			demo.VoidNoArgs();
			demo.VoidThreeStringArgs("", "", "");
		}

		[Test]
		public void Overrideing_ToString()
		{
			MockRepository mocks = new MockRepository();
			ObjectThatOverrideToString oid = (ObjectThatOverrideToString)
				mocks.StrictMock(typeof (ObjectThatOverrideToString));
			Expect.On(oid).Call(oid.ToString()).Return("bla");
			mocks.ReplayAll();
			Assert.AreEqual("bla", oid.ToString());
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof (AssertionException), "ErrorMessage")]
		public void CallbackThatThrows()
		{
			demo = (IDemo) mocks.StrictMock(typeof (IDemo));
			demo.VoidNoArgs();
			LastCall.Callback(new DelegateDefinations.NoArgsDelegate(ThrowFromCallback));
			mocks.ReplayAll();
			demo.VoidNoArgs();

		}

		#region Private Methods

		private static void RecordOrdered(MockRepository mocks, IDemo demo)
		{
			using (mocks.Ordered())
			{
				demo.ReturnStringNoArgs();
				LastCall.On(demo).Return(null);
				demo.VoidNoArgs();
				LastCall.On(demo).Repeat.Twice();
				demo.VoidStringArg("Hello");
				demo.VoidStringArg("World");
			}
		}

		#endregion

		private bool CallMethodOnDemo()
		{
			demo.VoidStringArg("Ayende");
			return true;
		}

		private bool ThrowFromCallback()
		{
			Assert.Fail("ErrorMessage");
			return true;
		}

		public class ObjectThatOverrideToString
		{
			public override string ToString()
			{
				return base.ToString ();
			}
		}
	}
}