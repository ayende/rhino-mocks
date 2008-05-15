using System;
using MbUnit.Framework;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_JudahG
	{
		public interface IView
		{
			int? Foo { get; set; }
		}

		[Test]
		public void IsMatching()
		{
			MockRepository mocks = new MockRepository();
			IView view = mocks.StrictMock<IView>();
			using (mocks.Record())
			{
				view.Foo = null;
				Predicate<int> alwaysReturnsTrue = delegate(int input)
				{
					return true;
				};
				LastCall.Constraints(Is.Matching(alwaysReturnsTrue));
			}
			using (mocks.Playback())
			{
				view.Foo = 1;
			}
		}
	}
}