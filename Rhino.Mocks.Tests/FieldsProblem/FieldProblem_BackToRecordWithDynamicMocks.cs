using System;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_BackToRecordWithDynamicMocks
    {
        [Test]
        public void BackToRecordOnADynamicMock()
        {
            MockRepository repository = new MockRepository();
            ITest test = (ITest)repository.DynamicMock(typeof(ITest));

            test.DoSomething(1);

            repository.BackToRecord(test);

            test.DoSomething(2);

            repository.ReplayAll();

            test.DoSomething(2);
            test.DoSomething(3);

            repository.VerifyAll();
        }

        public interface ITest
        {
            void DoSomething(int number);
        }
    }

}
