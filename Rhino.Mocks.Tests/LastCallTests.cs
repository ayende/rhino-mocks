using System;
using MbUnit.Framework;
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
		[ExpectedException(typeof (InvalidOperationException), "Invalid call, the last call has been used or no call has been made.")]
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
