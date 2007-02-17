using System;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_75_MockingExpcetion
	{
		[Test]
		public void MockingException()
		{
			MockRepository mocks = new MockRepository();
			InvalidOperationException mock = (InvalidOperationException)mocks.CreateMock(typeof(InvalidOperationException));
			Assert.IsNotNull(mock, "Should be able to create a mocked exception");
		}
	}
}
