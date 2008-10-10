using MbUnit.Framework;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Raju
    {
        public class A
        {
            public int a { get; set;}
            public int b { get; set; }
        }
        public interface MyInterface
        {
            int retValue(A a);
        }

        public class MyClass : MyInterface
        {
            public virtual int retValue(A a)
            {
                int i = 5;
                return i;
            }
        }
        [Test]
        public void TestMethod1()
        {
            MockRepository mock = new MockRepository();

            MyInterface myInterface = mock.StrictMock<MyInterface>();

            MyInterface myClass = new MyClass();

            A a = new A();
            a.a = 10;
            a.b = 12;

            myInterface.retValue(a);
            LastCall.Return(5).Constraints(
                Property.Value("a",10) && Property.Value("b",12)
                );
            mock.ReplayAll();

            int ret = myInterface.retValue(a);
            mock.VerifyAll();
            Assert.IsTrue(ret == 5);
        }
    }
}