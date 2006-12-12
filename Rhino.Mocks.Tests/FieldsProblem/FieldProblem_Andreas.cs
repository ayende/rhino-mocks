using NUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{

	[TestFixture]
	public class FieldProblem_Andreas
	{
		[Test]
		public void BackToRecordAll_EraseAllRecordedExpectations()
		{
			MockRepository repository = new MockRepository();
			TestedClass mockObject = (TestedClass)repository.CreateMock(typeof(TestedClass));

			mockObject.AnyMethod();
			repository.BackToRecordAll();
			mockObject.AnyMethod();

			repository.ReplayAll();
			mockObject.AnyMethod();
			repository.VerifyAll();

		}
	}

	public class TestedClass
	{
		public virtual void AnyMethod()
		{

		}
	}
}
