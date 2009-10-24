using System;
using System.Collections.Generic;
using Xunit;
using Rhino.Mocks;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	using Exceptions;

	
	public class FieldProblem_Stefan
	{
		// This test fixture relates to ploblems when ignoring arguments on generic method calls when the type is a struct (aka value type).
		// With reference types - such as String - there is no problem.
		// It has nothing to do with ordering or not -> but if you do not use an ordered mock recorder, then the error msg is not helpful.


		[Fact]
		public void ShouldIgnoreArgumentsOnGenericCallWhenTypeIsStruct()
		{
			// setup
			MockRepository mocks = new MockRepository();
			ISomeService m_SomeServiceMock = mocks.StrictMock<ISomeService>();
			SomeClient sut = new SomeClient(m_SomeServiceMock);

			using (mocks.Ordered())
			{
				Expect.Call(delegate
				{
					m_SomeServiceMock.DoSomething<string>(null, null);
				});
				LastCall.IgnoreArguments();

				Expect.Call(delegate
				{
					m_SomeServiceMock.DoSomething<DateTime>(null, default(DateTime));  // can't use null here, because it's a value type!
				});
				LastCall.IgnoreArguments();

			}
			mocks.ReplayAll();

			// test
			sut.DoSomething();

			// verification
			mocks.VerifyAll();

			// cleanup
			m_SomeServiceMock = null;
			sut = null;
		}

		[Fact]
		public void UnexpectedCallToGenericMethod()
		{
			MockRepository mocks = new MockRepository();
			ISomeService m_SomeServiceMock = mocks.StrictMock<ISomeService>();
			m_SomeServiceMock.DoSomething<string>(null, "foo");
			mocks.ReplayAll();
			Assert.Throws<ExpectationViolationException>(
				@"ISomeService.DoSomething<System.Int32>(null, 5); Expected #0, Actual #1.
ISomeService.DoSomething<System.String>(null, ""foo""); Expected #1, Actual #0.",
				() => m_SomeServiceMock.DoSomething<int>(null, 5));
		}

		[Fact]
		public void IgnoreArgumentsAfterDo()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.DynamicMock<IDemo>();
			bool didDo = false;
			demo.VoidNoArgs();
			LastCall
                .Do(SetToTrue(out didDo))
				.IgnoreArguments();

			mocks.ReplayAll();

			demo.VoidNoArgs();
			Assert.True(didDo, "Do has not been executed!");

			mocks.VerifyAll();
		}
		
		private delegate void PlaceHolder();

        private PlaceHolder SetToTrue(out bool didDo)
        {
			didDo = true;
            return delegate { };
        }
	}

	public interface ISomeService
	{
		void DoSomething<T>(string key, T someObj);
	}


	internal class SomeClient
	{
		private readonly ISomeService m_SomeSvc;

		public SomeClient(ISomeService someSvc)
		{
			m_SomeSvc = someSvc;
		}

		public void DoSomething()
		{
			m_SomeSvc.DoSomething<string>("string.test", "some string");

			m_SomeSvc.DoSomething<DateTime>("struct.test", DateTime.Now);

		}
	}
}