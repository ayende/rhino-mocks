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
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Tests.Callbacks;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class LastCallTests
	{
		private MockRepository mocks;
		private IDemo demo;
		private bool delegateWasCalled;

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "The object 'System.Object' is not a mocked object.")]
		public void LastCallOnNonMockObjectThrows()
		{
			try
			{
				LastCall.On(new object());
			}
			finally
			{
				mocks.ReplayAll(); //for the tear down
			}
		}

		[Test]
		public void LastCallConstraints()
		{
			mocks.ReplayAll();//we aren't using this, because we force an exception, which will be re-thrown on verify()
			
			MockRepository seperateMocks = new MockRepository();
			demo = (IDemo)seperateMocks.CreateMock(typeof (IDemo));
			demo.StringArgString("");
			LastCall.Constraints(Is.Null());
			LastCall.Return("aaa").Repeat.Twice();
			seperateMocks.ReplayAll();
			Assert.AreEqual("aaa",demo.StringArgString(null));

			try
			{
				demo.StringArgString("");
				Assert.Fail("Exception expected");
			}
			catch(Exception e)
			{
				Assert.AreEqual("IDemo.StringArgString(\"\"); Expected #0, Actual #1.\r\nIDemo.StringArgString(equal to null); Expected #2, Actual #1.",e.Message);
			}
		}

        [Test]
        public void LastCallCallOriginalMethod()
        {
            CallOriginalMethodFodder comf1 = (CallOriginalMethodFodder)mocks.DynamicMock(typeof(CallOriginalMethodFodder));
            CallOriginalMethodFodder comf2 = (CallOriginalMethodFodder)mocks.DynamicMock(typeof(CallOriginalMethodFodder));
            comf2.TheMethod();
            LastCall.CallOriginalMethod();

            mocks.ReplayAll();

            comf1.TheMethod();
            Assert.AreEqual(false, comf1.OriginalMethodCalled);

            comf2.TheMethod();
            Assert.AreEqual(true, comf2.OriginalMethodCalled);
        }

		[Test]
		[ExpectedException(typeof(ExpectationViolationException),"CallOriginalMethodFodder.TheMethod(); Expected #2, Actual #1.")]
		public void LastCallOriginalMethod_WithExpectation()
		{
			MockRepository mockRepository = new MockRepository();
			CallOriginalMethodFodder comf1 = (CallOriginalMethodFodder)mockRepository.DynamicMock(typeof(CallOriginalMethodFodder));
            CallOriginalMethodFodder comf2 = (CallOriginalMethodFodder)mockRepository.DynamicMock(typeof(CallOriginalMethodFodder));
            comf2.TheMethod();
			LastCall.CallOriginalMethod(OriginalCallOptions.CreateExpectation)
				.Repeat.Twice();

            mockRepository.ReplayAll();

            comf1.TheMethod();
            Assert.AreEqual(false, comf1.OriginalMethodCalled);

            comf2.TheMethod();
            Assert.AreEqual(true, comf2.OriginalMethodCalled);
  
			mockRepository.VerifyAll();
		}

        public class CallOriginalMethodFodder
        {
            private bool mOriginalMethodCalled;

	        public bool OriginalMethodCalled
	        {
		        get { return mOriginalMethodCalled;}
	        }

            public virtual void TheMethod()
            {
                mOriginalMethodCalled = true;
            }
        }

		[Test]
		public void LastCallCallback()
		{
			demo.VoidNoArgs();
			delegateWasCalled = false;
			LastCall.Callback(new DelegateDefinations.NoArgsDelegate(delegateCalled));
			mocks.ReplayAll();

			demo	.VoidNoArgs();
			Assert.IsTrue(delegateWasCalled);
		}

		private bool delegateCalled()
		{
			delegateWasCalled = true;
			return true;
		}

		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
			demo = (IDemo) mocks.CreateMock(typeof (IDemo));
		}

		[TearDown]
		public void Teardown()
		{
			mocks.ReplayAll();
			mocks.VerifyAll();
		}

		[Test]
		public void LastCallReturn()
		{
			demo.ReturnIntNoArgs();
			LastCall.Return(5);
			mocks.ReplayAll();
			Assert.AreEqual(5, demo.ReturnIntNoArgs());
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Invalid call, the last call has been used or no call has been made (did you make a call to a non virtual method?).")]
		public void NoLastCall()
		{
			try
			{
				LastCall.Return(null);
			}
			finally
			{
				mocks.ReplayAll(); //for the tear down
			}

		}

		[Test]
		[ExpectedException(typeof (Exception), "Bla!")]
		public void LastCallThrow()
		{
			demo.VoidNoArgs();
			LastCall.Throw(new Exception("Bla!"));
			mocks.ReplayAll();
			demo.VoidNoArgs();
		}

		[Test]
		public void LastCallRepeat()
		{
			demo.VoidNoArgs();
			LastCall.Repeat.Twice();
			mocks.ReplayAll();
			demo.VoidNoArgs();
			demo.VoidNoArgs();
		}

		[Test]
		public void LastCallIgnoreArguments()
		{
			demo.VoidStringArg("hello");
			LastCall.IgnoreArguments();
			mocks.ReplayAll();
			demo.VoidStringArg("bye");
		}


	}
}
