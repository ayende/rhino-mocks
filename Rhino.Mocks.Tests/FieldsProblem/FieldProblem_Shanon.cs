using MbUnit.Framework;
using RhinoMocksCPPInterfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Shanon
	{
		[Test]
		[Ignore("ModOpts in Reflection Emits - delayed for now")]
		public void CanMockInterfaceWithMethodsHavingModOpt()
		{
			MockRepository mocks = new MockRepository();
			IHaveMethodWithModOpts mock = mocks.CreateMock<IHaveMethodWithModOpts>();
			Assert.IsNotNull(mock);
		}
	}
}
