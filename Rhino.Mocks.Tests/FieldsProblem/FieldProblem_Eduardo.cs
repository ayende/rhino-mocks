namespace Rhino.Mocks.Tests.FieldsProblem
{
	using MbUnit.Framework;

	[TestFixture]
	public class FieldProblem_Eduardo
	{
		[Test]
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

		[Test]
		public void CanSetExpectationOnReadWritePropertyUsingAAASyntax()
		{
			var demo = MockRepository.GenerateMock<IDemo>();

			demo.Expect(x => x.Prop).SetPropertyWithArgument("Eduardo");

			demo.Prop = "Eduardo";

			demo.VerifyAllExpectations();
		}
	}
}