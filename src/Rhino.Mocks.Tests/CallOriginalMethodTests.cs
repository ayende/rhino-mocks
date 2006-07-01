using System;

namespace Rhino.Mocks.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class CallOriginalMethodTests
    {

        [Test]
        public void CallOriginalMethodOnPropGetAndSet()
        {
            MockRepository mocks = new MockRepository();
            MockingClassesTests.DemoClass demo = (MockingClassesTests.DemoClass)
                mocks.CreateMock(typeof(MockingClassesTests.DemoClass));

            SetupResult.For(demo.Prop).CallOriginalMethod();
            SetupResult.For(demo.Prop = 0).CallOriginalMethod();

            mocks.ReplayAll();

            for (int i = 0; i < 10; i++)
            {
                demo.Prop = i;
                Assert.AreEqual(i, demo.Prop);
            }
            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Can't use CallOriginalMethod on method ReturnIntNoArgs because the method is abstract.")]
        public void CantCallOriginalMethodOnInterface()
        {
            MockRepository mocks = new MockRepository();
            IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
            SetupResult.For(demo.ReturnIntNoArgs()).CallOriginalMethod();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Can't use CallOriginalMethod on method Six because the method is abstract.")]
        public void CantCallOriginalMethodOnAbstractMethod()
        {
            MockRepository mocks = new MockRepository();
            MockingClassesTests.AbstractDemo demo = (MockingClassesTests.AbstractDemo)mocks.CreateMock(typeof(MockingClassesTests.AbstractDemo));
            SetupResult.For(demo.Six()).CallOriginalMethod();
        }

    }
}
