using System;
using System.Text;
using NUnit.Framework;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class HandlingProperties
    {
        IDemo demo;
        MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            demo = (IDemo)mocks.CreateMock(typeof(IDemo));
        }

        [Test]
        public void PropertyBehaviorForSingleProperty()
        {
            Expect.Call(demo.Prop).PropertyBehavior();
            mocks.ReplayAll();
            for (int i = 0; i < 49; i++)
            {
                demo.Prop = "ayende" + i;
                Assert.AreEqual("ayende" + i, demo.Prop);
            }
            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Last method call was not made on a setter or a getter")]
        public void ExceptionIfLastMethodCallIsNotProperty()
        {
            Expect.Call(demo.EnumNoArgs()).PropertyBehavior();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Property must be read/write")]
        public void ExceptionIfPropHasOnlyGetter()
        {
            Expect.Call(demo.ReadOnly).PropertyBehavior();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Property must be read/write")]
        public void ExceptionIfPropHasOnlySetter()
        {
            Expect.Call(demo.WriteOnly).PropertyBehavior();
        }

        [Test]
        public void IndexedPropertiesSupported()
        {
            IWithIndexers x = (IWithIndexers)mocks.CreateMock(typeof(IWithIndexers));
            Expect.Call(x[1]).PropertyBehavior();
            Expect.Call(x["",1]).PropertyBehavior();
            mocks.ReplayAll();

            x[1] = 10;
            x[10] = 100;
            Assert.AreEqual(10, x[1]);
            Assert.AreEqual(100, x[10]);

            x["1", 2] = "3";
            x["2", 3] = "5";
            Assert.AreEqual("3", x["1", 2]);
            Assert.AreEqual("5", x["2", 3]);

            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Can't return a value for property Item because no value was set and the Property return a value type.")]
        public void IndexPropertyWhenValueTypeAndNotFoundThrows()
        {
            IWithIndexers x = (IWithIndexers)mocks.CreateMock(typeof(IWithIndexers));
            Expect.Call(x[1]).PropertyBehavior();
            mocks.ReplayAll();
            int dummy =  x[1];
        }

        [Test]
        public void IndexPropertyWhenRefTypeAndNotFoundReturnNull()
        {
            IWithIndexers x = (IWithIndexers)mocks.CreateMock(typeof(IWithIndexers));
            Expect.Call(x["",3]).PropertyBehavior();
            mocks.ReplayAll();
            Assert.IsNull(x["", 2]);
        }

        public interface IWithIndexers
        {
            int this[int x] { get; set; }

            string this[string n, int y] { get; set; } 
        }
    }
}
