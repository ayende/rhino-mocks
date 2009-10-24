#if DOTNET35
namespace Rhino.Mocks.Tests.FieldsProblem
{
	using Xunit;

	
	public class FieldProblem_Eduardo
	{
		[Fact]
		public void CanSetExpectationOnReadWritePropertyUsingRecordPlaybackSyntax()
		{
			var mocks = new MockRepository();
			var demo = mocks.DynamicMock<IDemo>();

			using (mocks.Record())
			{
				demo.Expect(x => x.Prop).SetPropertyWithArgument("Eduardo");
			}

			using (mocks.Playback())
			{
				demo.Prop = "Eduardo";
			}
		}

		[Fact]
		public void CanSetExpectationOnReadWritePropertyUsingAAASyntax()
		{
			var demo = MockRepository.GenerateMock<IDemo>();

			demo.Expect(x => x.Prop).SetPropertyWithArgument("Eduardo");

			demo.Prop = "Eduardo";

			demo.VerifyAllExpectations();
		}
	}
}
#endif