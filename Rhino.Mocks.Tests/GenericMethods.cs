using System;
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class GenericMethods
	{
		[Test]
		public void CanCreateMockOfInterfaceWithGenericMethod()
		{
			MockRepository mocks = new MockRepository();
			mocks.CreateMock<IFactory>();
		}

		[Test]
		public void CanSetExpectationsOnInterfaceWithGenericMethod()
		{
			MockRepository mocks = new MockRepository();
			IFactory factory = mocks.CreateMock<IFactory>();
			Expect.Call(factory.Create<string>()).Return("working?");
			mocks.ReplayAll();
			string result = factory.Create<string>();
			Assert.AreEqual("working?",result, "Should have worked, hm..." );
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException),@"IFactory.Create<System.Int32>(); Expected #1, Actual #1.
IFactory.Create<System.String>(); Expected #1, Actual #0." )]
		public void WillGetErrorIfCallingMethodWithDifferentGenericArgument()
		{
			MockRepository mocks = new MockRepository();
			IFactory factory = mocks.CreateMock<IFactory>();
			Expect.Call(factory.Create<string>()).Return("working?");
			mocks.ReplayAll();
			factory.Create<int>();
		}


		[Test]
		[ExpectedException(typeof(InvalidOperationException),"Type 'System.Int32' doesn't match the return type 'System.String' for method 'IFactory.Create<System.String>();'")]
		public void WillGiveErrorIfThereIsTypeMismatchInGenericParameters()
		{
			MockRepository mocks = new MockRepository();
			IFactory factory = mocks.CreateMock<IFactory>();
			Expect.Call(factory.Create<string>()).Return(1);
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IFactory.Create<System.String>(); Expected #1, Actual #0.")]
		public void WillGiveErrorIfMissingCallToGenericMethod()
		{
			MockRepository mocks = new MockRepository();
			IFactory factory = mocks.CreateMock<IFactory>();
			Expect.Call(factory.Create<string>()).Return("working?");
			mocks.ReplayAll();
			mocks.VerifyAll();
		}

	}



	public interface IFactory
	{
		T Create<T>();
	}
}