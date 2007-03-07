using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	internal class InternalClass
	{
	}

	[TestFixture]
	public class InternalClassMockingFixture
	{
		[Test]
		public void MockInternalClass()
		{
			MockRepository mocker = new MockRepository();
			InternalClass mockInternalClass = mocker.CreateMock<InternalClass>();

			Assert.IsNotNull(mockInternalClass);
		}
	}
}