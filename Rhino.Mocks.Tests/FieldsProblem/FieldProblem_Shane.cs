namespace Rhino.Mocks.Tests.FieldsProblem
{
	using System;
	using System.Diagnostics;
	using Exceptions;
	using Xunit;

	
	public class FieldProblem_Shane
	{
		[Fact]
		public void WillMerge_UnorderedRecorder_WhenRecorderHasSingleRecorderInside()
		{
			MockRepository mocks = new MockRepository();
			ICustomer customer = mocks.StrictMock<ICustomer>();

			CustomerMapper mapper = new CustomerMapper();

			using (mocks.Record())
			using (mocks.Ordered())
			{
				Expect.Call(customer.Id).Return(0);

				customer.IsPreferred = true;
			}

			Assert.Throws<ExpectationViolationException>("Unordered method call! The expected call is: 'Ordered: { ICustomer.get_Id(); }' but was: 'ICustomer.set_IsPreferred(True);'", () =>
			{
				using (mocks.Playback())
				{
					mapper.MarkCustomerAsPreferred(customer);
				}
			});
		}
	}

	public interface ICustomer
	{
		int Id { get; }

		bool IsPreferred { get; set; }
	}

	public class CustomerMapper
	{
		public void MarkCustomerAsPreferred(ICustomer customer)
		{
			customer.IsPreferred = true;

			int id = customer.Id;
		}
	}
}