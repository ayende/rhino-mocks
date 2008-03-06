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
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class MockRepositoryTests
	{
		private MockRepository mocks;
		private IDemo demo;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			demo = this.mocks.CreateMock(typeof(IDemo)) as IDemo;
		}

		[Test]
		public void CreatesNewMockObject()
		{
			Assert.IsNotNull(demo, "Couldn't creates a demo mock.");
		}

		[Test]
		public void CallMethodOnMockObject()
		{
			demo.ReturnStringNoArgs();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IDemo.ReturnStringNoArgs(); Expected #1, Actual #0.")]
		public void RecordWithBadReplayCauseException()
		{
			demo.ReturnStringNoArgs();
			LastCall.On(demo).Return(null);
			mocks.Replay(demo);
			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IDemo.ReturnStringNoArgs(); Expected #2, Actual #1.")]
		public void RecordTwoMethodsButReplayOneCauseException()
		{
			demo.ReturnStringNoArgs();
			LastCall.On(demo).Return(null).Repeat.Twice();
			mocks.Replay(demo);
			demo.ReturnStringNoArgs();
			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof(ObjectNotMockFromThisRepositoryException), "The object is not a mock object that belong to this repository.")]
		public void CallingReplayOnNonMockThrows()
		{
			MockRepository mocks = new MockRepository();
			mocks.Replay(new object());
		}

		[Test]
		[ExpectedException(typeof(ObjectNotMockFromThisRepositoryException), "The object is not a mock object that belong to this repository.")]
		public void CallingVerifyOnNonMockThrows()
		{
			MockRepository mocks = new MockRepository();
			mocks.Verify(new object());
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "This action is invalid when the mock object is in replay state.")]
		public void TryingToReplayMockMoreThanOnceThrows()
		{
			mocks.Replay(demo);
			mocks.Replay(demo);
		}

		[Test]
		public void CallingReplayAndThenReplayAll()
		{
			mocks.Replay(demo);
			mocks.ReplayAll();
		}


		[Test]
		public void CallingVerifyAndThenVerifyAll()
		{
			mocks.ReplayAll();
			mocks.Verify(demo);
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "This action is invalid when the mock object is in record state.")]
		public void CallingVerifyWithoutReplayFirstCauseException()
		{
			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "This action is invalid when the mock object is in verified state.")]
		public void UsingVerifiedObjectThrows()
		{
			mocks.Replay(demo);
			mocks.Verify(demo);
			demo.ReturnIntNoArgs();
		}


		[Test]
		[ExpectedException(typeof(InvalidOperationException), "This action is invalid when the mock object is in replay state.")]
		public void CallingLastMethodOptionsOnReplay()
		{
			demo.VoidNoArgs();
			mocks.Replay(demo);
			LastCall.On(demo);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Previous method 'IDemo.StringArgString(\"\");' requires a return value or an exception to throw.")]
		public void NotClosingMethodBeforeReplaying()
		{
			demo.StringArgString("");
			mocks.Replay(demo);
		}

		[Test]
		public void GetmocksFromProxy()
		{
			IMockedObject mockedObj = demo as IMockedObject;
			Assert.IsNotNull(mockedObj, "Could not get IMockObject from mocked object");
			MockRepository MockRepository = mockedObj.Repository;
			Assert.IsNotNull(MockRepository);
			Assert.AreSame(mocks, MockRepository);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "There is no matching last call on this object. Are you sure that the last call was a virtual or interface method call?")]
		public void CallingLastCallWithoutHavingLastCallThrows()
		{
			LastCall.On(demo);
		}

		[Test]
		public void SetReturnValue()
		{
			demo.ReturnStringNoArgs();
			string retVal = "Ayende";
			LastCall.On(demo).Return(retVal);
			mocks.Replay(demo);
			Assert.AreEqual(retVal, demo.ReturnStringNoArgs());
			mocks.Verify(demo);
		}

		[Test]
		public void SetReturnValueAndNumberOfRepeats()
		{
			demo.ReturnStringNoArgs();
			string retVal = "Ayende";
			LastCall.On(demo).Return(retVal).Repeat.Twice();
			mocks.Replay(demo);
			Assert.AreEqual(retVal, demo.ReturnStringNoArgs());
			Assert.AreEqual(retVal, demo.ReturnStringNoArgs());
			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException), "Reserved value, must be zero")]
		public void SetMethodToThrow()
		{
			demo.VoidStringArg("test");
			LastCall.On(demo).Throw(new ArgumentException("Reserved value, must be zero"));
			mocks.Replay(demo);
			demo.VoidStringArg("test");
		}

		[Test]
		public void SettingMethodToThrowTwice()
		{
			demo.VoidStringArg("test");
			string exceptionMessage = "Reserved value, must be zero";
			LastCall.On(demo).Throw(new ArgumentException(exceptionMessage)).Repeat.Twice();
			mocks.Replay(demo);
			for (int i = 0; i < 2; i++)
			{
				try
				{
					demo.VoidStringArg("test");
					Assert.Fail("Expected exception");
				}
				catch (ArgumentException e)
				{
					Assert.AreEqual(exceptionMessage, e.Message);
				}
			}
		}

		[Test]
		public void ReturnningValueType()
		{
			demo.ReturnIntNoArgs();
			LastCall.On(demo).Return(2);
			mocks.Replay(demo);
			Assert.AreEqual(2, demo.ReturnIntNoArgs());
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Previous method 'IDemo.ReturnIntNoArgs();' requires a return value or an exception to throw.")]
		public void CallingSecondMethodWithoutSetupRequiredInfoOnFirstOne()
		{
			demo.ReturnIntNoArgs();
			demo.ReturnIntNoArgs();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Type 'System.DateTime' doesn't match the return type 'System.Int32' for method 'IDemo.ReturnIntNoArgs();'")]
		public void TryingToSetUnrelatedTypeAsReturnValueThrows()
		{
			demo.ReturnIntNoArgs();
			LastCall.On(demo).Return(new DateTime());
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Type 'null' doesn't match the return type 'System.Int32' for method 'IDemo.ReturnIntNoArgs();'")]
		public void ReturnNullForValueType()
		{
			demo.ReturnIntNoArgs();
			LastCall.On(demo).Return(null);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Type 'System.Int32' doesn't match the return type 'System.Void' for method 'IDemo.VoidNoArgs();'")]
		public void ReturnValueForVoidMethod()
		{
			demo.VoidNoArgs();
			LastCall.On(demo).Return(3);
		}

		[Test]
		public void ReturnDerivedType()
		{
			demo.EnumNoArgs();
			LastCall.On(demo).Return(DemoEnum.Demo);
		}

		[Test]
		public void SetExceptionAndThenSetReturn()
		{
			demo.EnumNoArgs();
			LastCall.On(demo).Throw(new Exception());
			demo.EnumNoArgs();
			LastCall.On(demo).Return(DemoEnum.Demo);
			mocks.Replay(demo);
			try
			{
				demo.EnumNoArgs();
				Assert.Fail("Expected exception");
			}
			catch (Exception)
			{
			}
			DemoEnum d = DemoEnum.NonDemo;
			d = (DemoEnum)demo.EnumNoArgs();
			Assert.AreEqual(d, DemoEnum.Demo);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Can set only a single return value or exception to throw or delegate to execute on the same method call.")]
		public void SetReturnValueAndExceptionThrows()
		{
			demo.EnumNoArgs();
			LastCall.On(demo).Throw(new Exception());
			LastCall.On(demo).Return(DemoEnum.Demo);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Can set only a single return value or exception to throw or delegate to execute on the same method call.")]
		public void SetExceptionAndThenThrows()
		{
			demo.EnumNoArgs();
			LastCall.On(demo).Throw(new Exception());
			LastCall.On(demo).Return(DemoEnum.Demo);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Can set only a single return value or exception to throw or delegate to execute on the same method call.")]
		public void SetTwoReturnValues()
		{
			demo.EnumNoArgs();
			LastCall.On(demo).Return(DemoEnum.Demo);
			LastCall.On(demo).Return(DemoEnum.Demo);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Can set only a single return value or exception to throw or delegate to execute on the same method call.")]
		public void SetTwoExceptions()
		{
			demo.EnumNoArgs();
			LastCall.On(demo).Throw(new Exception());
			LastCall.On(demo).Throw(new Exception());

		}

		[Test]
		public void ExpectMethodOnce()
		{
			demo.EnumNoArgs();
			LastCall.On(demo).Return(DemoEnum.NonDemo).Repeat.Once();
			mocks.Replay(demo);
			DemoEnum d = (DemoEnum)demo.EnumNoArgs();
			Assert.AreEqual(d, DemoEnum.NonDemo);
			try
			{
				demo.EnumNoArgs();
				Assert.Fail("Expected exception");
			}
			catch (ExpectationViolationException e)
			{
				Assert.AreEqual("IDemo.EnumNoArgs(); Expected #1, Actual #2.", e.Message);
			}
		}

		[Test]
		public void ExpectMethodAlways()
		{
			demo.EnumNoArgs();
			LastCall.On(demo).Return(DemoEnum.NonDemo).Repeat.Any();
			mocks.Replay(demo);
			demo.EnumNoArgs();
			demo.EnumNoArgs();
			demo.EnumNoArgs();
			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IDemo.VoidStringArg(\"World\"); Expected #0, Actual #1.\r\nIDemo.VoidStringArg(\"Hello\"); Expected #1, Actual #0.")]
		public void DifferentArgumentsCauseException()
		{
			demo.VoidStringArg("Hello");
			mocks.Replay(demo);
			demo.VoidStringArg("World");
		}

		[Test]
		public void VerifyingArguments()
		{
			demo.VoidStringArg("Hello");
			mocks.Replay(demo);
			demo.VoidStringArg("Hello");
			mocks.Verify(demo);
		}

		[Test]
		public void IgnoreArgument()
		{
			demo.VoidStringArg("Hello");
			LastCall.On(demo).IgnoreArguments();
			mocks.Replay(demo);
			demo.VoidStringArg("World");
			mocks.Verify(demo);
		}

		[Test]
		public void IgnoreArgsAndReturnValue()
		{
			demo.StringArgString("Hello");
			string objToReturn = "World";
			LastCall.On(demo).IgnoreArguments().Repeat.Twice().Return(objToReturn);
			mocks.Replay(demo);
			Assert.AreEqual(objToReturn, demo.StringArgString("foo"));
			Assert.AreEqual(objToReturn, demo.StringArgString("bar"));
			mocks.Verify(demo);
		}

		[Test]
		public void RepeatThreeTimes()
		{
			demo.StringArgString("Hello");
			string objToReturn = "World";
			LastCall.On(demo).IgnoreArguments().Repeat.Times(3).Return(objToReturn);
			mocks.Replay(demo);
			Assert.AreEqual(objToReturn, demo.StringArgString("foo"));
			Assert.AreEqual(objToReturn, demo.StringArgString("bar"));
			Assert.AreEqual(objToReturn, demo.StringArgString("bar"));
			mocks.Verify(demo);
		}

		[Test]
		public void RepeatOneToThreeTimes()
		{
			demo.StringArgString("Hello");
			string objToReturn = "World";
			LastCall.On(demo).IgnoreArguments().Repeat.Times(1, 3).Return(objToReturn);
			mocks.Replay(demo);
			Assert.AreEqual(objToReturn, demo.StringArgString("foo"));
			Assert.AreEqual(objToReturn, demo.StringArgString("bar"));
			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof(Exception), "Ugh! It's alive!")]
		public void ThrowingExceptions()
		{
			demo.StringArgString("Ayende");
			LastCall.On(demo).Throw(new Exception("Ugh! It's alive!")).IgnoreArguments();
			mocks.Replay(demo);
			demo.StringArgString(null);
		}


		[Test]
		[ExpectedException(typeof(Exception), "Ugh! It's alive!")]
		public void ThrowingExceptionsWhenOrdered()
		{
			using (mocks.Ordered())
			{
				demo.StringArgString("Ayende");
				LastCall.On(demo).Throw(new Exception("Ugh! It's alive!")).IgnoreArguments();
			}
			mocks.Replay(demo);
			demo.StringArgString(null);
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IDemo.VoidNoArgs(); Expected #0, Actual #1.")]
		public void ExpectationExceptionWhileUsingDisposableThrowTheCorrectExpectation()
		{
			mocks.Replay(demo);
			demo.VoidNoArgs();
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IDemo.VoidNoArgs(); Expected #0, Actual #1.")]
		public void MockObjectThrowsForUnexpectedCall()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
			mocks.ReplayAll();
			demo.VoidNoArgs();
			mocks.VerifyAll();
		}



		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IDemo.VoidNoArgs(); Expected #0, Actual #1.")]
		public void MockObjectThrowsForUnexpectedCall_WhenVerified_IfFirstExceptionWasCaught()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
			mocks.ReplayAll();
			try
			{
				demo.VoidNoArgs();
			}
			catch (Exception) { }
			mocks.VerifyAll();
		}

		[Test]
		public void DyamicMockAcceptUnexpectedCall()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = (IDemo)mocks.DynamicMock(typeof(IDemo));
			mocks.ReplayAll();
			demo.VoidNoArgs();
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void RepositoryThrowsWithConstructorArgsForMockInterface()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo), "Foo");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void RepositoryThrowsWithConstructorArgsForMockDelegate()
		{
			MockRepository mocks = new MockRepository();
			EventHandler handler = (EventHandler)mocks.CreateMock(typeof(EventHandler), "Foo");
		}

		[Test]
		[ExpectedException(typeof(MissingMethodException), "Can't find a constructor with matching arguments")]
		public void RepositoryThrowsWithWrongConstructorArgsForMockClass()
		{
			MockRepository mocks = new MockRepository();
			// There is no constructor on object that takes a string
			// parameter, so this should fail.
			object o = mocks.CreateMock(typeof(object), "Foo");
		}

		[Test]
		[ExpectedArgumentNullException]
		public void IsInReplayModeThrowsWhenPassedNull()
		{
			mocks.IsInReplayMode(null);
		}

		[Test]
		[ExpectedArgumentException]
		public void IsInReplayModeThrowsWhenPassedNonMock()
		{
			mocks.IsInReplayMode(new object());
		}

		[Test]
		public void IsInReplayModeReturnsTrueWhenMockInReplay()
		{
			mocks.Replay(demo);

			Assert.IsTrue(mocks.IsInReplayMode(demo));
		}

		[Test]
		public void IsInReplayModeReturnsFalseWhenMockInRecord()
		{
			Assert.IsFalse(mocks.IsInReplayMode(demo));
		}

		#region Implementation

		private enum DemoEnum
		{
			Demo,
			NonDemo
		}

		#endregion
	}
}
