using System;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_JohanIndexProp 
    {
        [Test]
        public void CreateMockWithIndexedProp()
        {
            MockRepository mocks = new MockRepository();
            IWithIndexedProperty index = (IWithIndexedProperty)mocks.CreateMock(typeof(IWithIndexedProperty));
            Expect.Call(index.get_Foo("Blah")).Return(5);
            index.set_Foo("Foo",2);
            mocks.ReplayAll();

            Assert.AreEqual(5, index.get_Foo("Blah"));
            index.set_Foo("Foo", 2);

            mocks.VerifyAll();

        }
    }

    public interface IWithIndexedProperty
    {
        int get_Foo(string key);
        void set_Foo(string key, int val);
    }
}
