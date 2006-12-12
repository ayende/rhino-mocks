using System;
using NUnit.Framework;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class ExpectTests
	{
		private MockRepository mocks;
		private IDemo demo;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			demo = this.mocks.CreateMock(typeof (IDemo)) as IDemo;
		}

		[TearDown]
		public void Teardown()
		{
			mocks.VerifyAll();
		}

		[Test]
		public void CanExpect()
		{
			Expect.On(demo).Call(demo.Prop).Return("Ayende");
			mocks.ReplayAll();
			Assert.AreEqual("Ayende", demo.Prop);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "The object 'System.Object' is not a mocked object.")]
		public void PassNonMock()
		{
			try
			{
				Expect.On(new object());
			}
			finally
			{
				mocks.ReplayAll(); //for the tear down
			}

		}

		[Test]
		public void ExpectCallNormal()
		{
			Expect.Call(demo.Prop).Return("ayende");
			mocks.ReplayAll();
			Assert.AreEqual("ayende", demo.Prop);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Invalid call, the last call has been used or no call has been made.")]
		public void ExpectWhenNoCallMade()
		{
			try
			{
				Expect.Call(null);
			}
			finally
			{
				mocks.ReplayAll(); //for the tear down
			}
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Invalid call, the last call has been used or no call has been made.")]
		public void ExpectOnReplay()
		{
			Expect.Call(demo.Prop).Return("ayende");
			mocks.ReplayAll();
			Assert.AreEqual("ayende", demo.Prop);
			Expect.Call(null);
		}
	}
}