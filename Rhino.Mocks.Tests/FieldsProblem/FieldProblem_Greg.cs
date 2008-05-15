using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Greg
    {
        private MockRepository _mockRepository;

        [SetUp]
        public void Initialize()
        {
            _mockRepository = new MockRepository();
        }

        [Test]
        public void IgnoreArguments()
        {
            IFoo myFoo = _mockRepository.StrictMock<IFoo>();
            IBar<int> myBar = _mockRepository.StrictMock<IBar<int>>();

            using(_mockRepository.Record())
            using (_mockRepository.Ordered())
            {
                Expect.Call(myFoo.RunBar(myBar)).IgnoreArguments().Return(true);
            }

            using (_mockRepository.Playback())
            {
                Example<int> myExample = new Example<int>(myFoo, myBar);
                bool success = myExample.ExampleMethod();
                Assert.IsTrue(success);
            }
        }
    }

    public class Example<T>
    {
        private readonly IBar<T> _bar;
        private readonly IFoo _foo;

        public Example(IFoo foo, IBar<T> bar)
        {
            _foo = foo;
            _bar = bar;
        }

        public bool ExampleMethod()
        {
            bool success = _foo.RunBar(_bar);
            return success;
        }
    }

    public interface IFoo
    {
        bool RunBar<T>(IBar<T> barObject);
    }

    public interface IBar<T>
    {
        void BarMethod(T paramToBarMethod);
    }

    public class Foo : IFoo
    {
        //When Foo is mocked, this method returns FALSE!!!

        #region IFoo Members

        public bool RunBar<T>(IBar<T> barObject)
        {
            return true;
        }

        #endregion
    }

    public class Bar<T> : IBar<T>
    {
        #region IBar<T> Members

        public void BarMethod(T paramToBarMethod)
        {
            //nothing important
        }

        #endregion
    }
}