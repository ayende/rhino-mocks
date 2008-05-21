using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	using Exceptions;

	[TestFixture]
	public class FieldProblem_Rob
	{
		[Test]
		[ExpectedException(typeof(ExpectationViolationException),
			"IDemo.VoidNoArgs(); Expected #0, Actual #1.")]
		public void CanFailIfCalledMoreThanOnceUsingDynamicMock()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.DynamicMock<IDemo>();
			demo.VoidNoArgs();
			LastCall.Repeat.Once();// doesn't realy matter
			demo.VoidNoArgs();
			LastCall.Repeat.Never();

			mocks.ReplayAll();

			demo.VoidNoArgs();//should work

			demo.VoidNoArgs();// will fail
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException),@"ISomeSystem.GetFooFor<Rhino.Mocks.Tests.FieldsProblem.UnexpectedBar>(""foo""); Expected #1, Actual #1.
ISomeSystem.GetFooFor<Rhino.Mocks.Tests.FieldsProblem.ExpectedBar>(""foo""); Expected #1, Actual #0.")]
		public void Ayende_View_On_Mocking()
		{
			MockRepository mocks = new MockRepository();
			ISomeSystem mockSomeSystem = mocks.StrictMock<ISomeSystem>();

			using (mocks.Record())
			{
				Expect.Call(mockSomeSystem.GetFooFor<ExpectedBar>("foo"))
					.Return(new List<ExpectedBar>());
			}

			using (mocks.Playback())
			{
				ExpectedBarPerformer cut = new ExpectedBarPerformer(mockSomeSystem);
				cut.DoStuffWithExpectedBar("foo");
			}
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
