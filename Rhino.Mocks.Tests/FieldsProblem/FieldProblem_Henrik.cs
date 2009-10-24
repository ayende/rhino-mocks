#if DOTNET35
namespace Rhino.Mocks.Tests.FieldsProblem
{
	using System;
	using Xunit;

	
	public class FieldProblem_Henrik
	{
		[Fact]
		public void Trying_to_mock_null_instance_should_fail_with_descriptive_error_message()
		{
			Assert.Throws<ArgumentNullException>("You cannot mock a null instance\r\nParameter name: mock", 
				() => RhinoMocksExtensions.Expect<object>(null, x => x.ToString()));
		}
	}
}
#endif