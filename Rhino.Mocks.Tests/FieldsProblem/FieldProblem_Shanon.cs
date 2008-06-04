using MbUnit.Framework;
using RhinoMocksCPPInterfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Shanon
	{
		[Test]
		public void CanMockInterfaceWithMethodsHavingModOpt()
		{
			MockRepository mocks = new MockRepository();
			IHaveMethodWithModOpts mock = mocks.StrictMock<IHaveMethodWithModOpts>();
			Assert.IsNotNull(mock);
		}
	}
}
