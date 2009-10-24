using System;
using System.Web.Security;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	
	public class FieldProblem_StevenS : IDisposable
	{
		private MockRepository mocks;
		private MembershipProvider myMembershipProvider;

		public FieldProblem_StevenS()
		{
			mocks = new MockRepository();
			myMembershipProvider = mocks.StrictMock<MembershipProvider>();
		} 

        [Fact]
        public void LoadFromUserId()
        {
            SetupResult.For(myMembershipProvider.Name).Return("Foo");

            Expect.Call(myMembershipProvider.GetUser("foo",false)).Return(null);

            mocks.ReplayAll();

        	myMembershipProvider.GetUser("foo", false);
        }

		[Fact]
		public void LoadFromUserId_Object()
		{
			SetupResult.For(myMembershipProvider.Name).Return("Foo");

			object foo = "foo";
			Expect.Call(myMembershipProvider.GetUser(foo, false)).Return(null);

			mocks.ReplayAll();

			myMembershipProvider.GetUser(foo, false);
		} 


		public void Dispose()
		{
			mocks.VerifyAll();
		}
	}
}