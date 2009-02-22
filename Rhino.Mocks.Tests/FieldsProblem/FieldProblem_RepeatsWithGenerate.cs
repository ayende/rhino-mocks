using MbUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests.FieldsProblem
{

	[TestFixture]
	public class FieldProblem_RepeatsWithGenerate
	{
		[Test]
        [ExpectedException(typeof(ExpectationViolationException), "IRepeatsWithGenerate.GetMyIntValue(); Expected #1, Actual #2.")]
        public void RepeatTimes_Fails_When_Called_More_Then_Expected()
        {
            var mockRepository = new MockRepository();

		    var interfaceMock = mockRepository.StrictMock<IRepeatsWithGenerate>();

            Expect.Call(interfaceMock.GetMyIntValue())
                .Repeat.Times(1)
		        .Return(4);

            mockRepository.ReplayAll();

		    interfaceMock.GetMyIntValue();
		    interfaceMock.GetMyIntValue();

            mockRepository.Verify(interfaceMock);
        }

		[Test]
        [ExpectedException(typeof(ExpectationViolationException), "IRepeatsWithGenerate.GetMyIntValue(); Expected #2, Actual #1.")]
        public void RepeatTimes_Works_When_Called_Less_Then_Expected()
        {
		    var mockRepository = new MockRepository();

            var interfaceMock = mockRepository.StrictMock<IRepeatsWithGenerate>();

            Expect.Call(interfaceMock.GetMyIntValue())
                .Repeat.Times(2)
		        .Return(4);

            mockRepository.ReplayAll();

		    interfaceMock.GetMyIntValue();

            mockRepository.Verify(interfaceMock);
        }
	}

	public interface IRepeatsWithGenerate
	{
	    int GetMyIntValue();
	}
}