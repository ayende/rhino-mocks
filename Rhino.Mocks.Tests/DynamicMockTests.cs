using System;
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class DynamicMockTests
	{
		MockRepository mocks;
		IDemo demo;

		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
			demo = (IDemo)mocks.DynamicMock(typeof(IDemo));
		}

		[TearDown]
		public void Teardown()
		{
			mocks.VerifyAll();
		}

		[Test]
		public void CanCallUnexpectedMethodOnDynamicMock()
		{
			mocks.ReplayAll();
			Assert.AreEqual(0,demo.ReturnIntNoArgs());
		}

		[Test]
		public void CanSetupExpectations()
		{
			Expect.Call(demo.ReturnIntNoArgs()).Return(30);
			mocks.ReplayAll();
			Assert.AreEqual(30,demo.ReturnIntNoArgs(),"Expected call didn't return setup value");
			Assert.AreEqual(0,demo.ReturnIntNoArgs(),"Unexpected call return non default value");
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException),"IDemo.ReturnIntNoArgs(); Expected #1, Actual #0." )]
		public void ExpectationExceptionWithDynamicMock()
		{
			Expect.Call(demo.ReturnIntNoArgs()).Return(30);
			mocks.ReplayAll();
			Assert.IsNull(demo.ReturnStringNoArgs());
			mocks.Verify(demo);	
		}

		[Test]
		public void SetupResultWorksWithDynamicMocks()
		{
			SetupResult.For(demo.StringArgString("Ayende")).Return("Rahien");
			mocks.ReplayAll();
			for (int i = 0; i < 43; i++)
			{
				Assert.AreEqual("Rahien",demo.StringArgString("Ayende"));
				Assert.IsNull(demo.StringArgString("another"));
			}
		}

		[Test]
		public void ExpectNeverForDyanmicMock()
		{
			Expect.Call(demo.ReturnIntNoArgs()).Repeat.Never();
			mocks.ReplayAll();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException),"IDemo.ReturnIntNoArgs(); Expected #0, Actual #1.")]
		public void ExpectNeverForDyanmicMockThrowsIfOccurs()
		{
			Expect.Call(demo.ReturnIntNoArgs()).Repeat.Never();
			mocks.ReplayAll();
			demo.ReturnIntNoArgs();
		}
	}
}
