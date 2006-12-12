using System;

namespace Rhino.Mocks.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class MockingAbstractClass
	{
		private MockRepository mocks;

		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
		}

		[TearDown]
		public void Teardown()
		{
			mocks.VerifyAll();
		}

		[Test]
		public void MockAbsPropertyGetter()
		{
			AbsCls ac = (AbsCls)mocks.CreateMock(typeof(AbsCls));
			Expect.Call(ac.AbPropGet).Return("n");
			mocks.ReplayAll();
			Assert.AreEqual("n", ac.AbPropGet);
		}

		[Test]
		public void MockAbsPropertySetter()
		{
			AbsCls ac = (AbsCls)mocks.CreateMock(typeof(AbsCls));
			Expect.Call(ac.AbPropSet = "n");
			mocks.ReplayAll();
			ac.AbPropSet = "n";
		}


		[Test]
		public void MockAbsProp()
		{
			AbsCls ac = (AbsCls)mocks.CreateMock(typeof(AbsCls));
			Expect.Call(ac.AbProp = "n");
			Expect.Call(ac.AbProp).Return("u");
			mocks.ReplayAll();
			ac.AbProp = "n";
			Assert.AreEqual("u", ac.AbProp);
		}

		[Test]
		public void MockAbstractMethod()
		{
			AbsCls ac = (AbsCls)mocks.CreateMock(typeof(AbsCls));
			Expect.Call(ac.Method()).Return(45);
			mocks.ReplayAll();
			Assert.AreEqual(45, ac.Method());
	
		}

		public abstract class AbsCls
		{
			public abstract string AbPropGet { get; }

			public abstract string AbPropSet { set; }

			public abstract string AbProp { get; set; }

			public abstract int Method();

		}
	}
}
