namespace Rhino.Mocks.Tests.FieldsProblem
{
	using Xunit;

	
	public class SetupResultAndIgnoreArguments
	{
		[Fact]
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

			Assert.Equal(2, fetcher.GetUsersWithCriteriaLike("foo").Length);
		}

		[Fact]
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

			Assert.Equal(2, fetcher.GetUsersWithCriteriaLike("foo").Length);
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