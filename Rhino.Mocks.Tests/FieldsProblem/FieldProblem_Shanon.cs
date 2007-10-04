using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	using MbUnit.Framework;
	using RhinoMocksCPPInterfaces;

	[TestFixture]
	public class FieldProblem_Shanon
	{
		[Test]
		public void CanMockInterfaceWithMethodsHavingModOpt()
		{
			MockRepository mocks = new MockRepository();
			IHaveMethodWithModOpts mock = mocks.CreateMock<IHaveMethodWithModOpts>();
			Assert.IsNotNull(mock);
		}
	}
}
