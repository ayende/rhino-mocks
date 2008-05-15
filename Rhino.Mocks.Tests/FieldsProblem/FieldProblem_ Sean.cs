using System.Security.Permissions;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem__Sean
	{
		[Test]
		[Ignore("Not sure what the problem is, and don't know enough about CAS to try to figure it out")]
		public void CanMockMethodWithEnvironmentPermissions()
		{
			MockRepository mocks = new MockRepository();
			IEmployeeRepository employeeRepository = mocks.StrictMock<IEmployeeRepository>();
			IEmployee employee = mocks.StrictMock<IEmployee>();
			
			using (mocks.Record())
			{
				employeeRepository.GetEmployeeDetails("ayende");
				LastCall.Return(employee);
			}

			using(mocks.Playback())
			{
				IEmployee actual = employeeRepository.GetEmployeeDetails("ayende");
				Assert.AreEqual(employee, actual);
			}
		}

	}

	public interface IEmployeeRepository
	{
		[EnvironmentPermission(SecurityAction.LinkDemand)]
		IEmployee GetEmployeeDetails(string username);
	}

	public interface IEmployee
	{
	}
}
