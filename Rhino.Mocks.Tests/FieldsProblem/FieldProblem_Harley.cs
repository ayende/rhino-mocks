#if DOTNET35
using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    
    public class FieldProblem_Harley
    {

        public delegate void ChangeTestEvent(bool value);
        public interface ClassToMock
        {
            event ChangeTestEvent ChangeTestProperty;
        }

        [Fact]
        public void TestSampleMatrixChanged()
        {
            var repository = new MockRepository();
            var mockTestClass = repository.DynamicMock<ClassToMock>();
            mockTestClass.ChangeTestProperty += null;
            var fireChangeTestProperty = LastCall.IgnoreArguments().GetEventRaiser();
            new ClassRaisingException(mockTestClass);

            repository.ReplayAll();

			Assert.Throws<ArgumentOutOfRangeException>(() => fireChangeTestProperty.Raise(true));
        }
    }


    public class ClassRaisingException
    {
        public ClassRaisingException(FieldProblem_Harley.ClassToMock eventRaisingClass)
        {
            eventRaisingClass.ChangeTestProperty += handleTestEvent;
        }

        public void handleTestEvent(bool value)
        {
            if (value)
                throw new ArgumentOutOfRangeException();
        }
    }

}
#endif
