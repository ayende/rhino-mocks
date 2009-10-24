using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    
    public class FieldProblem_Christian
    {
        [Fact]
        public void PropertiesWillBehaveLikeProperties()
        {
            MockRepository mocks = new MockRepository();
            TestObject testObject = mocks.Stub<TestObject>();

            mocks.ReplayAll();

            Assert.Equal(0, testObject.IntProperty);
        }
    }

    public class TestObject
    {
        private int _intProperty;

        public virtual int IntProperty
        {
            get { return _intProperty; }
            set { _intProperty = value; }
        }
    }
}