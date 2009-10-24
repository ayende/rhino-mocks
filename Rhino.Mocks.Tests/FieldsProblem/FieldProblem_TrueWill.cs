#if DOTNET35
using System;
using Xunit;
using Rhino.Mocks;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    
    public class FieldProblem_TrueWill
    {
        [Fact]
        public void ReadWritePropertyBug1()
        {
            ISomeThing thing = MockRepository.GenerateStub<ISomeThing>();
            thing.Number = 21;
            thing.Stub(x => x.Name).Return("Bob");
            Assert.Equal(thing.Number, 21);
            // Fails - calling Stub on anything after
            // setting property resets property to default.
        }

        [Fact]
        public void ReadWritePropertyBug2()
        {
            ISomeThing thing = MockRepository.GenerateStub<ISomeThing>();
        	Assert.Throws<InvalidOperationException>(
        		@"You are trying to set an expectation on a property that was defined to use PropertyBehavior.
Instead of writing code such as this: mockObject.Stub(x => x.SomeProperty).Return(42);
You can use the property directly to achieve the same result: mockObject.SomeProperty = 42;",
        		() => thing.Stub(x => x.Number).Return(21));
            // InvalidOperationException :
            // Invalid call, the last call has been used...
            // This broke a test on a real project when a
            // { get; } property was changed to { get; set; }.
        }
    }

    public interface ISomeThing
    {
        string Name { get; }

        int Number { get; set; }
    }

}
#endif