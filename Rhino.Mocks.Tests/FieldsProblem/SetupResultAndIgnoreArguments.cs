namespace Rhino.Mocks.Tests.FieldsProblem
{
	using MbUnit.Framework;

	[TestFixture]
	public class SetupResultAndIgnoreArguments
	{
		[Test]
		public void CanUseSetupResultAndIgnoreArguments_WhenUsingUnorderedBlock()
		{
			MockRepository mocks = new MockRepository();
			IFetcher fetcher = mocks.DynamicMock<IFetcher>();

			using (mocks.Unordered())
			{
				SetupResult.For(fetcher.GetUsersWithCriteriaLike(null)).IgnoreArguments().Return(
					new Student[] {new Student(), new Student()});
			}

			mocks.ReplayAll();

			Assert.AreEqual(2, fetcher.GetUsersWithCriteriaLike("foo").Length);
		}

		[Test]
		public void CanUseSetupResultAndIgnoreArguments_WhenUsingOrderedBlock()
		{
			MockRepository mocks = new MockRepository();
			IFetcher fetcher = mocks.DynamicMock<IFetcher>();

			using (mocks.Ordered())
			{
				SetupResult.For(fetcher.GetUsersWithCriteriaLike(null)).IgnoreArguments().Return(
					new Student[] { new Student(), new Student() });
			}

			mocks.ReplayAll();

			Assert.AreEqual(2, fetcher.GetUsersWithCriteriaLike("foo").Length);
		}
	}

	public interface IFetcher
	{
		Student[] GetUsersWithCriteriaLike(string likeString);
	}

	public class Student
	{
	}
}