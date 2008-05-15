using System;
using System.Collections.ObjectModel;
using MbUnit.Framework;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_MichaelC
	{
		[Test]
		public void EventRaiser_ShouldRaiseEvent_OnlyOnce()
		{
			MockRepository mocks = new MockRepository();
			IWithEvent mock = mocks.StrictMock<IWithEvent>();
			int countOne = 0;
			int countTwo = 0;
			mock.Load += null;
			IEventRaiser raiser = LastCall.IgnoreArguments()
				.Repeat.Twice()
				.GetEventRaiser();
			mocks.ReplayAll();
			mock.Load += delegate { countOne++; };
			mock.Load += delegate { countTwo++; };
			raiser.Raise(this, EventArgs.Empty);
			Assert.AreEqual(1, countOne);
			Assert.AreEqual(1, countTwo);
			raiser.Raise(this, EventArgs.Empty);
			Assert.AreEqual(2, countOne);
			Assert.AreEqual(2, countTwo);
			raiser.Raise(this, EventArgs.Empty);
			Assert.AreEqual(3, countOne);
			Assert.AreEqual(3, countTwo);
		}

	}
}