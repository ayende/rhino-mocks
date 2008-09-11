#if DOTNET35
namespace Rhino.Mocks.Tests.FieldsProblem
{
	using System;
	using MbUnit.Framework;

	[TestFixture]
	public class FieldProblem_Henrik
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException), "You cannot mock a null instance\r\nParameter name: mock")]
		public void Trying_to_mock_null_instance_should_fail_with_descriptive_error_message()
		{
			RhinoMocksExtensions.Expect<object>(null, x => x.ToString());
		}
	}
}
#endif