using MbUnit.Framework;
using mshtml;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Luke
	{
		[Test]
		public void CanMockIE()
		{
			MockRepository mockRepository = new MockRepository();
			IHTMLEventObj2 mock = mockRepository.CreateMock<IHTMLEventObj2>();
			Assert.IsNotNull(mock);
		}
	}

}