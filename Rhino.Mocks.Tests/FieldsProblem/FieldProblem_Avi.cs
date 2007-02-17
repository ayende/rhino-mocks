using System;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Avi
    {
        [Test]
        public void CallNonThrowingProtectedCtor()
        {
            MockRepository mocks = new MockRepository();

            ClassWithThrowingCtor mockClass1 = (ClassWithThrowingCtor)mocks.CreateMock(typeof(ClassWithThrowingCtor), CallOptions.DontCallCtor);
        }

    }

    public enum CallOptions
    {
        DontCallCtor
    }

    public class ClassWithThrowingCtor
    {
        //this is used to mark the ctor that the mock objects will call.
        protected ClassWithThrowingCtor(CallOptions tag)
        {
        }

        public ClassWithThrowingCtor()
        {
            throw new NotImplementedException("NIY, and I don't want my mock to get here!");
        }
    }
#if dotNet2
    [TestFixture]
    public class RhinoDynamicMockOfGeneric
    {
        [Test]
        public void createDynamicMockOfGeneric()
        {
            MockRepository mocks = new MockRepository();
            genericClass<int> mockA =
                mocks.DynamicMock<genericClass<int>>();
            mocks.Replay(mockA);
        }

        [Test]
        public void TestMockOnGenericWithDifferentTypes()
        {
            MockRepository mocks = new MockRepository();
            IComparable<int> mock1 = mocks.CreateMock<IComparable<int>>();
            IComparable<bool> mock2 = mocks.CreateMock<IComparable<bool>>();
        }

        [Test]
        public void createDynamicMockOfGenericAgain()
        {
            MockRepository mocks = new MockRepository();
            genericClass<int> mockA =
                mocks.DynamicMock<genericClass<int>>();
        }


        public class genericClass<T>
        {
            public T Field;
        }

    }
#endif

}

