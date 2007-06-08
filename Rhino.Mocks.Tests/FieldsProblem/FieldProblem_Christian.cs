using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Christian
    {
        [Test]
        public void PropertiesWillBehaveLikeProperties()
        {
            MockRepository mocks = new MockRepository();
            TestObject testObject = mocks.Stub<TestObject>();

            mocks.ReplayAll();

            Assert.AreEqual(0, testObject.IntProperty);
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