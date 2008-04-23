using System;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Harley
    {

        public delegate void ChangeTestEvent(bool value);
        public interface ClassToMock
        {
            event ChangeTestEvent ChangeTestProperty;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSampleMatrixChanged()
        {
            var repository = new MockRepository();
            var mockTestClass = repository.DynamicMock<ClassToMock>();
            mockTestClass.ChangeTestProperty += null;
            var fireChangeTestProperty = LastCall.IgnoreArguments().GetEventRaiser();
            new ClassRaisingException(mockTestClass);

            repository.ReplayAll();
            fireChangeTestProperty.Raise(true);
            repository.VerifyAll();
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