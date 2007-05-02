using MbUnit.Framework;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Robert
	{
		public interface IView
		{
			void RedrawDisplay(object something);
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IView.RedrawDisplay(\"blah\"); Expected #4, Actual #5" )]
		public void CorrectResultForExpectedWhenUsingTimes()
		{
			MockRepository mocks = new MockRepository();
			IView view = mocks.CreateMock<IView>();
			using (mocks.Record())
			{
				view.RedrawDisplay(null);
				LastCall.Repeat.Times(4).IgnoreArguments();
			}
			using(mocks.Playback())
			{
				for (int i = 0; i < 5; i++)
				{
					view.RedrawDisplay("blah");
				}
			}
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IView.RedrawDisplay(\"blah\"); Expected #3 - 4, Actual #5" )]
		public void CorrectResultForExpectedWhenUsingTimesWithRange()
		{
			MockRepository mocks = new MockRepository();
			IView view = mocks.CreateMock<IView>();
			using (mocks.Record())
			{
				view.RedrawDisplay(null);
				LastCall.Repeat.Times(3,4).IgnoreArguments();
			}
			using(mocks.Playback())
			{
				for (int i = 0; i < 5; i++)
				{
					view.RedrawDisplay("blah");
				}
			}
		}
	}
}