using System.Web.Security;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_StevenS
	{
		private MockRepository mocks;
		private MembershipProvider myMembershipProvider;

		[SetUp]
		public void MyTestInitialize()
		{
			mocks = new MockRepository();
			myMembershipProvider = mocks.StrictMock<MembershipProvider>();
		} 

        [Test]
        public void LoadFromUserId()
        {
            SetupResult.For(myMembershipProvider.Name).Return("Foo");

            Expect.Call(myMembershipProvider.GetUser("foo",false)).Return(null);

            mocks.ReplayAll();

        	myMembershipProvider.GetUser("foo", false);
        }

		[Test]
		public void LoadFromUserId_Object()
		{
			SetupResult.For(myMembershipProvider.Name).Return("Foo");

			object foo = "foo";
			Expect.Call(myMembershipProvider.GetUser(foo, false)).Return(null);

			mocks.ReplayAll();

			myMembershipProvider.GetUser(foo, false);
		} 


		[TearDown]
		public void MyTestTearDown()
		{
			mocks.VerifyAll();
		}
	}
}