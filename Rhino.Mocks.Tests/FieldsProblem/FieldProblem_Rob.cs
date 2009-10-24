using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	using Exceptions;

	
	public class FieldProblem_Rob
	{
		[Fact]
		public void CanFailIfCalledMoreThanOnceUsingDynamicMock()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.DynamicMock<IDemo>();
			demo.VoidNoArgs();
			LastCall.Repeat.Once();// doesn't realy matter
			demo.VoidNoArgs();
			LastCall.Repeat.Never();

			mocks.ReplayAll();

			Assert.Throws<ExpectationViolationException>("IDemo.VoidNoArgs(); Expected #0, Actual #1.", demo.VoidNoArgs);
		}

		[Fact]
		public void Ayende_View_On_Mocking()
		{
			MockRepository mocks = new MockRepository();
			ISomeSystem mockSomeSystem = mocks.StrictMock<ISomeSystem>();

			using (mocks.Record())
			{
				Expect.Call(mockSomeSystem.GetFooFor<ExpectedBar>("foo"))
					.Return(new List<ExpectedBar>());
			}

			Assert.Throws<ExpectationViolationException>(
				@"ISomeSystem.GetFooFor<Rhino.Mocks.Tests.FieldsProblem.UnexpectedBar>(""foo""); Expected #1, Actual #1.
ISomeSystem.GetFooFor<Rhino.Mocks.Tests.FieldsProblem.ExpectedBar>(""foo""); Expected #1, Actual #0.",
				() =>
				{
					using (mocks.Playback())
					{
						ExpectedBarPerformer cut = new ExpectedBarPerformer(mockSomeSystem);
						cut.DoStuffWithExpectedBar("foo");
					}
				}
				);

			
		}
	}

	public interface ISomeSystem
	{
		List<TBar> GetFooFor<TBar>(string key) where TBar : Bar;
	}
	public class Bar { }
	public class ExpectedBar : Bar { }
	public class UnexpectedBar : Bar { }

	public class ExpectedBarPerformer
	{
		ISomeSystem system;
		
		public ExpectedBarPerformer(ISomeSystem system)
		{
			this.system = system;
		}

		public void DoStuffWithExpectedBar(string p)
		{
			IList<UnexpectedBar> list = system.GetFooFor<UnexpectedBar>(p);

		}
	}
}
