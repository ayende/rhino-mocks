using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests
{
    public delegate object ObjectDelegateWithNoParams();

    [TestFixture]
    public class MockingDelegatesTests
    {
        private MockRepository mocks;
        private delegate object ObjectDelegateWithNoParams();
        private delegate void VoidDelegateWithParams(string a);
        private delegate string StringDelegateWithParams(int a, string b);
        private delegate int IntDelegateWithRefAndOutParams(ref int a, out string b);

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void CallingMockedDelegatesWithoutOn()
        {
            ObjectDelegateWithNoParams d1 = (ObjectDelegateWithNoParams)mocks.CreateMock(typeof(ObjectDelegateWithNoParams));
            Expect.Call(d1()).Return(1);

            mocks.ReplayAll();

            Assert.AreEqual(1, d1());
        }

        [Test]
        public void MockTwoDelegatesWithTheSameName()
        {
            ObjectDelegateWithNoParams d1 = (ObjectDelegateWithNoParams)mocks.CreateMock(typeof(ObjectDelegateWithNoParams));
            Tests.ObjectDelegateWithNoParams d2 = (Tests.ObjectDelegateWithNoParams)mocks.CreateMock(typeof(Tests.ObjectDelegateWithNoParams));

            Expect.On(d1).Call(d1()).Return(1);
            Expect.On(d2).Call(d2()).Return(2);

            mocks.ReplayAll();

            Assert.AreEqual(1, d1());
            Assert.AreEqual(2, d2());

            mocks.VerifyAll();
        }

        [Test]
        public void MockObjectDelegateWithNoParams()
        {
            ObjectDelegateWithNoParams d = (ObjectDelegateWithNoParams)mocks.CreateMock(typeof(ObjectDelegateWithNoParams));

            Expect.On(d).Call(d()).Return("abc");
            Expect.On(d).Call(d()).Return("def");

            mocks.Replay(d);

            Assert.AreEqual("abc", d());
            Assert.AreEqual("def", d());

            try
            {
                d();
                Assert.Fail("Expected an expectation violation to occur.");
            }
            catch (ExpectationViolationException)
            {
                // Expected.
            }

            mocks.VerifyAll();
        }

        [Test]
        public void MockVoidDelegateWithNoParams()
        {
            VoidDelegateWithParams d = (VoidDelegateWithParams)mocks.CreateMock(typeof(VoidDelegateWithParams));
            d("abc");
            d("efg");

            mocks.Replay(d);

            d("abc");
            d("efg");

            try
            {
                d("hij");
                Assert.Fail("Expected an expectation violation to occur.");
            }
            catch (ExpectationViolationException)
            {
                // Expected.
            }

            mocks.VerifyAll();
        }

        [Test]
        public void MockStringDelegateWithParams()
        {
            StringDelegateWithParams d = (StringDelegateWithParams)mocks.CreateMock(typeof(StringDelegateWithParams));

            Expect.On(d).Call(d(1, "111")).Return("abc");
            Expect.On(d).Call(d(2, "222")).Return("def");

            mocks.Replay(d);

            Assert.AreEqual("abc", d(1, "111"));
            Assert.AreEqual("def", d(2, "222"));

            try
            {
                d(3, "333");
                Assert.Fail("Expected an expectation violation to occur.");
            }
            catch (ExpectationViolationException)
            {
                // Expected.
            }

            mocks.VerifyAll();
        }

        [Test]
        public void MockIntDelegateWithRefAndOutParams()
        {
            IntDelegateWithRefAndOutParams d = (IntDelegateWithRefAndOutParams)mocks.CreateMock(typeof(IntDelegateWithRefAndOutParams));

            int a = 3;
            string b = null;
            Expect.On(d).Call(d(ref a, out b)).Do(new IntDelegateWithRefAndOutParams(Return1_Plus2_A));

            mocks.Replay(d);

            Assert.AreEqual(1, d(ref a, out b));
            Assert.AreEqual(5, a);
            Assert.AreEqual("A", b);

            try
            {
                d(ref a, out b);
                Assert.Fail("Expected an expectation violation to occur.");
            }
            catch (ExpectationViolationException)
            {
                // Expected.
            }

            mocks.VerifyAll();
        }

        [Test]
        public void InterceptsDynamicInvokeAlso()
        {
            IntDelegateWithRefAndOutParams d = (IntDelegateWithRefAndOutParams)mocks.CreateMock(typeof(IntDelegateWithRefAndOutParams));

            int a = 3;
            string b = null;
            Expect.On(d).Call(d(ref a, out b)).Do(new IntDelegateWithRefAndOutParams(Return1_Plus2_A));

            mocks.Replay(d);

            object[] args = new object[] { 3, null };
            Assert.AreEqual(1, d.DynamicInvoke(args));
            Assert.AreEqual(5, args[0]);
            Assert.AreEqual("A", args[1]);

            try
            {
                d.DynamicInvoke(args);
                Assert.Fail("Expected an expectation violation to occur.");
            }
            catch (TargetInvocationException ex)
            {
                // Expected.
                Assert.IsTrue(ex.InnerException is ExpectationViolationException);
            }

            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Cannot mock the Delegate base type.")]
        public void DelegateBaseTypeCannotBeMocked()
        {
            mocks.CreateMock(typeof(Delegate));
        }

        private int Return1_Plus2_A(ref int a, out string b)
        {
            a += 2;
            b = "A";
            return 1;
        }

#if dotNet2
        [Test]
        public void GenericDelegate()
        {
            Action<int> action = mocks.CreateMock<Action<int>>();
            for (int i = 0; i < 10; i++)
            {
                action(i);
            }
            mocks.ReplayAll();
            ForEachFromZeroToNine(action);
            mocks.VerifyAll();
        }

        private void ForEachFromZeroToNine(Action<int> act)
        {
            for (int i = 0; i < 10; i++)
            {
                act(i);
            }
        }
#endif
    }
}
