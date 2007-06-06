using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Roy
    {
        [Test]
        public void StubNeverFailsTheTest()
        {
            MockRepository repository = new MockRepository();
            IGetResults resultGetter = repository.Stub<IGetResults>();
            using (repository.Record())
            {
                resultGetter.GetSomeNumber("a");
                LastCall.Return(1);
            }

            int result = resultGetter.GetSomeNumber("b");
            Assert.AreEqual(0, result);
            repository.VerifyAll(); //<- should not fail the test methinks
        }

        [Test]
        public void CanGetSetupResultFromStub()
        {
            MockRepository repository = new MockRepository();
            IGetResults resultGetter = repository.Stub<IGetResults>();
            using (repository.Record())
            {
                resultGetter.GetSomeNumber("a");
                LastCall.Return(1);
            }

            int result = resultGetter.GetSomeNumber("a");
            Assert.AreEqual(1, result);
            repository.VerifyAll(); 
        }
    }

    public interface IGetResults
    {
        int GetSomeNumber(string s);
    }
}