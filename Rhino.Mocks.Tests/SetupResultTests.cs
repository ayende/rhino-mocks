using System;
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class SetupResultTests
	{
		private MockRepository mocks;
		private IDemo demo;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			demo = this.mocks.CreateMock(typeof (IDemo)) as IDemo;
		}

        [Test]
        public void CanSetupResultForMethodAndIgnoreArgs()
        {
            SetupResult.For(demo.StringArgString(null)).Return("Ayende").IgnoreArguments();
            mocks.ReplayAll();
            Assert.AreEqual("Ayende", demo.StringArgString("a"));
            Assert.AreEqual("Ayende", demo.StringArgString("b"));
            mocks.VerifyAll();
            
        }
	    
		[Test]
		public void CanSetupResult()
		{
			SetupResult.For(demo.Prop).Return("Ayende");
			mocks.ReplayAll();
			Assert.AreEqual("Ayende", demo.Prop);
            mocks.VerifyAll();
		    
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Invalid call, the last call has been used or no call has been made.")]
		public void SetupResultForNoCall()
		{
			SetupResult.For(null);
		}

		[Test]
		public void SetupResultCanRepeatAsManyTimeAsItWant()
		{
			SetupResult.For(demo.Prop).Return("Ayende");
			mocks.ReplayAll();
			for (int i = 0; i < 30; i++)
			{
				Assert.AreEqual("Ayende", demo.Prop);
			}
            mocks.VerifyAll();
		    
		}

		[Test]
		public void SetupResultUsingOn()
		{
			SetupResult.On(demo).Call(demo.Prop).Return("Ayende");
			mocks.ReplayAll();
			for (int i = 0; i < 30; i++)
			{
				Assert.AreEqual("Ayende", demo.Prop);
			}
            mocks.VerifyAll();
		    
		}

		[Test]
		public void SetupResultUsingOrdered()
		{
			SetupResult.On(demo).Call(demo.Prop).Return("Ayende");
			using (mocks.Ordered())
			{
				demo.VoidNoArgs();
				LastCall.On(demo).Repeat.Twice();
			}
			mocks.ReplayAll();
			demo.VoidNoArgs();
			for (int i = 0; i < 30; i++)
			{
				Assert.AreEqual("Ayende", demo.Prop);
			}
			demo.VoidNoArgs();
            mocks.VerifyAll();
		    
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "The result for IDemo.get_Prop(); has already been setup.")]
		public void SetupResultForTheSameMethodTwiceCauseExcetion()
		{
			SetupResult.On(demo).Call(demo.Prop).Return("Ayende");
			SetupResult.On(demo).Call(demo.Prop).Return("Ayende");
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException),"IDemo.ReturnIntNoArgs(); Expected #0, Actual #1.")]
		public void ExpectNever()
		{
			demo.ReturnStringNoArgs();
			LastCall.Repeat.Never();
			mocks.ReplayAll();
			demo.ReturnIntNoArgs();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),"The result for IDemo.ReturnStringNoArgs(); has already been setup.")]
		public void ExpectNeverSetupTwiceThrows()
		{
			demo.ReturnStringNoArgs();
			LastCall.Repeat.Never();
			demo.ReturnStringNoArgs();
			LastCall.Repeat.Never();
			
		}
	}
}