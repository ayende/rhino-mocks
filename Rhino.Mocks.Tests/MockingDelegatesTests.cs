#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.IO;
using System.Reflection;
using MbUnit.Framework;
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
            ObjectDelegateWithNoParams d1 = (ObjectDelegateWithNoParams)mocks.StrictMock(typeof(ObjectDelegateWithNoParams));
            Expect.Call(d1()).Return(1);

            mocks.ReplayAll();

            Assert.AreEqual(1, d1());
        }

        [Test]
        public void MockTwoDelegatesWithTheSameName()
        {
            ObjectDelegateWithNoParams d1 = (ObjectDelegateWithNoParams)mocks.StrictMock(typeof(ObjectDelegateWithNoParams));
            Tests.ObjectDelegateWithNoParams d2 = (Tests.ObjectDelegateWithNoParams)mocks.StrictMock(typeof(Tests.ObjectDelegateWithNoParams));

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
            ObjectDelegateWithNoParams d = (ObjectDelegateWithNoParams)mocks.StrictMock(typeof(ObjectDelegateWithNoParams));

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
        }

        [Test]
        public void MockVoidDelegateWithNoParams()
        {
            VoidDelegateWithParams d = (VoidDelegateWithParams)mocks.StrictMock(typeof(VoidDelegateWithParams));
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
        }

        [Test]
        public void MockStringDelegateWithParams()
        {
            StringDelegateWithParams d = (StringDelegateWithParams)mocks.StrictMock(typeof(StringDelegateWithParams));

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
        }

        [Test]
        public void MockIntDelegateWithRefAndOutParams()
        {
            IntDelegateWithRefAndOutParams d = (IntDelegateWithRefAndOutParams)mocks.StrictMock(typeof(IntDelegateWithRefAndOutParams));

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

        }

        [Test]
        public void InterceptsDynamicInvokeAlso()
        {
            IntDelegateWithRefAndOutParams d = (IntDelegateWithRefAndOutParams)mocks.StrictMock(typeof(IntDelegateWithRefAndOutParams));

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
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Cannot mock the Delegate base type.")]
        public void DelegateBaseTypeCannotBeMocked()
        {
            mocks.StrictMock(typeof(Delegate));
        }

        private int Return1_Plus2_A(ref int a, out string b)
        {
            a += 2;
            b = "A";
            return 1;
        }

        [Test]
        public void GenericDelegate()
        {
            Action<int> action = mocks.StrictMock<Action<int>>();
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
    }
}
