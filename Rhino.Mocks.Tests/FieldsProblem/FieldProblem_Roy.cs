using System;
using Xunit;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    
    public class FieldProblem_Roy
    {
        [Fact]
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
            Assert.Equal(0, result);
            repository.VerifyAll(); //<- should not fail the test methinks
        }

        [Fact]
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
            Assert.Equal(1, result);
            repository.VerifyAll(); 
        }

        [Fact]
        public void CannotCallLastCallConstraintsMoreThanOnce()
        {
            MockRepository repository = new MockRepository();
            IGetResults resultGetter = repository.Stub<IGetResults>();
        	Assert.Throws<InvalidOperationException>(
        		"You have already specified constraints for this method. (IGetResults.GetSomeNumber(contains \"b\");)",
        		() =>
        		{
        			using (repository.Record())
        			{
        				resultGetter.GetSomeNumber("a");
        				LastCall.Constraints(Text.Contains("b"));
        				LastCall.Constraints(Text.Contains("a"));
        			}
        		});
        }
    }

    public interface IGetResults
    {
        int GetSomeNumber(string s);
    }
}