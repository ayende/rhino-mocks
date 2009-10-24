using Xunit;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    
    public class FieldProblem_Joshua
    {
        [Fact]
        public void The_value_of_a_variable_used_as_an_out_parameter_should_not_be_used_as_a_constraint_on_an_expectation()
        {
            MockRepository mockRepository = new MockRepository();
            ServiceBeingCalled service = mockRepository.StrictMock<ServiceBeingCalled>();
            const int theNumberToReturnFromTheServiceOutParameter = 20;
            
            using(mockRepository.Record())
            {
                int uninitialized;
                
                // Uncommenting the following line will make the test pass, because the expectation constraints will match up with the actual call.
                // However, the value of an out parameter cannot be used within a method value before it is set within the method value,
                // so the value going in really is irrelevant, and should therefore be ignored when evaluating constraints.
                // Even ReSharper will tell you "Value assigned is not used in any execution path" for the following line.
                
                //uninitialized = 42;

                // I understand I can do an IgnoreArguments() or Contraints(Is.Equal("key"), Is.Anything()), but I think the framework should take care of that for me

                Expect.Call(service.PopulateOutParameter("key", out uninitialized)).Return(null).OutRef(theNumberToReturnFromTheServiceOutParameter);
            }
            ObjectBeingTested testObject = new ObjectBeingTested(service);
            int returnedValue = testObject.MethodUnderTest();
            Assert.Equal(theNumberToReturnFromTheServiceOutParameter, returnedValue);
        }

        public class ObjectBeingTested
        {
            private ServiceBeingCalled service;

            public ObjectBeingTested(ServiceBeingCalled service)
            {
                this.service = service;
            }

            public int MethodUnderTest()
            {
                const int A_NUMBER_THAT_SHOULD_BE_IGNORED = 42;
                int thisShouldGetPopulatedByTheService = A_NUMBER_THAT_SHOULD_BE_IGNORED;
                service.PopulateOutParameter("key", out thisShouldGetPopulatedByTheService);
                return thisShouldGetPopulatedByTheService;
            }
        }

        public interface ServiceBeingCalled
        {
            object PopulateOutParameter(string anInputParameter, out int theOutputParameter);
        }
    }
}
