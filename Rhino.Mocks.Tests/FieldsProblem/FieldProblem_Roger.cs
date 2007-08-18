using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Roger
	{
		[Test]
		public void VerifyMockCanBeSetupWhenExternalInterfaceUsingInnerClassWithInternalScope()
		{
			MockRepository mocks = new MockRepository();
			ISomeInterface<InnerClass> target = mocks.CreateMock<ISomeInterface<InnerClass>>();
		}
	}

	internal class InnerClass
	{
	}

	public interface ISomeInterface<T> { }

}
