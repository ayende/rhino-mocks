#region license

// Copyright (c) 2007 Ivan Krivyakov (ivan@ikriv.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
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
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class ProxyMockTests
    {
        public class TestClass : MarshalByRefObject
        {
            public TestClass(string unused)
            {
                throw new InvalidCastException("Real method should never be called"); 
            }

            public void Method() 
            { 
                throw new InvalidCastException("Real method should never be called"); 
            }

            public int MethodReturningInt()
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public string MethodReturningString()
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public string MethodGettingParameters(int intParam, string stringParam)
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public int GenericMethod<T>(string parameter)
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public T GenericMethodReturningGenericType<T>(string parameter)
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public T GenericMethodWithGenericParam<T>( T parameter )
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public string StringProperty
            {
                get
                {
                    throw new InvalidCastException("Real method should never be called");
                }
                set
                {
                    throw new InvalidCastException("Real method should never be called");
                }
            }
        }

        public class GenericTestClass<T> : MarshalByRefObject
        {
            public int Method(T parameter)
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public U GenericMethod<U>(T parameter)
            {
                throw new InvalidCastException("Real method should never be called");
            }
        }

        [Test]
        public void CanMockVoidMethod()
        {
            MockRepository mocks = new MockRepository();
            TestClass t = (TestClass)mocks.CreateMock(typeof(TestClass));
            t.Method();
            mocks.ReplayAll();
            t.Method();
            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(ExpectationViolationException), "TestClass.Method(); Expected #0, Actual #1.")]
        public void ThrowOnUnexpectedVoidMethod()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.CreateMock(typeof(TestClass));
            mocks.ReplayAll();
            t.Method();
            mocks.VerifyAll();
        }

        [Test]
        public void CanMockMethodReturningInt()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.CreateMock(typeof(TestClass));
            Expect.Call(t.MethodReturningInt()).Return(42);
            mocks.ReplayAll();
            Assert.AreEqual(42, t.MethodReturningInt());
            mocks.VerifyAll();
        }

        [Test]
        public void CanMockMethodReturningString()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.CreateMock(typeof(TestClass));
            Expect.Call(t.MethodReturningString()).Return("foo");
            mocks.ReplayAll();
            Assert.AreEqual("foo", t.MethodReturningString());
            mocks.VerifyAll();
        }

        [Test]
        public void CanMockMethodGettingParameters()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.CreateMock(typeof(TestClass));
            Expect.Call(t.MethodGettingParameters(42, "foo")).Return("bar");
            mocks.ReplayAll();
            Assert.AreEqual("bar", t.MethodGettingParameters(42, "foo"));
            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(ExpectationViolationException),
            "TestClass.MethodGettingParameters(19, \"foo\"); Expected #0, Actual #1.\r\nTestClass.MethodGettingParameters(42, \"foo\"); Expected #1, Actual #0.")]
        public void CanRejectIncorrectParameters()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.CreateMock(typeof(TestClass));
            Expect.Call(t.MethodGettingParameters(42, "foo")).Return("bar");
            mocks.ReplayAll();
            Assert.AreEqual("bar", t.MethodGettingParameters(19, "foo"));
            mocks.VerifyAll();
        }

        [Test]
        public void CanMockPropertyGet()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.CreateMock(typeof(TestClass));
            Expect.Call(t.StringProperty).Return("foo");
            mocks.ReplayAll();
            Assert.AreEqual("foo", t.StringProperty);
            mocks.VerifyAll();
        }

        [Test]
        public void CanMockPropertySet()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.CreateMock(typeof(TestClass));
            t.StringProperty = "foo";
            mocks.ReplayAll();
            t.StringProperty = "foo";
            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(ExpectationViolationException),
            "TestClass.set_StringProperty(\"bar\"); Expected #0, Actual #1.\r\nTestClass.set_StringProperty(\"foo\"); Expected #1, Actual #0.")]
        public void CanRejectIncorrectPropertySet()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.CreateMock(typeof(TestClass));
            t.StringProperty = "foo";
            mocks.ReplayAll();
            t.StringProperty = "bar";
            mocks.VerifyAll();
        }

        [Test]
        public void CanMockGenericClass()
        {
            MockRepository mocks = new MockRepository();
			GenericTestClass<string> t = (GenericTestClass<string>)mocks.CreateMock(typeof(GenericTestClass<string>));
            Expect.Call(t.Method("foo")).Return(42);
            mocks.ReplayAll();
            Assert.AreEqual(42, t.Method("foo"));
            mocks.VerifyAll();
        }

        [Test]
        public void CanMockGenericMethod()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.CreateMock(typeof(TestClass));
            Expect.Call(t.GenericMethod<string>("foo")).Return(42);
            mocks.ReplayAll();
            Assert.AreEqual(42, t.GenericMethod<string>("foo"));
            mocks.VerifyAll();
        }

		[Test,ExpectedException(typeof(ExpectationViolationException),
@"TestClass.GenericMethod<System.Int32>(""foo""); Expected #1, Actual #1.
TestClass.GenericMethod<System.String>(""foo""); Expected #1, Actual #0.")]
		public void CanMockGenericMethod_WillErrorOnWrongType()
		{
			MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.CreateMock(typeof(TestClass));
			Expect.Call(t.GenericMethod<string>("foo")).Return(42);
			mocks.ReplayAll();
			Assert.AreEqual(42, t.GenericMethod<int>("foo"));
			mocks.VerifyAll();
		}

        [Test]
        public void CanMockGenericMethodReturningGenericType()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.CreateMock(typeof(TestClass));
            Expect.Call(t.GenericMethodReturningGenericType<string>("foo")).Return("bar");
            mocks.ReplayAll();
            Assert.AreEqual("bar", t.GenericMethodReturningGenericType<string>("foo"));
            mocks.VerifyAll();
        }

        [Test]
        public void CanMockGenericMethodWithGenericParam()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.CreateMock(typeof(TestClass));
            Expect.Call(t.GenericMethodWithGenericParam<string>("foo")).Return("bar");
            mocks.ReplayAll();
            Assert.AreEqual("bar", t.GenericMethodWithGenericParam("foo"));
            mocks.VerifyAll();
        }

        [Test]
        public void CanMockGenericMethodInGenericClass()
        {
            MockRepository mocks = new MockRepository();
			GenericTestClass<string> t = mocks.CreateMock<GenericTestClass<string>>();
            Expect.Call(t.GenericMethod<int>("foo")).Return(42);
            mocks.ReplayAll();
            Assert.AreEqual(42, t.GenericMethod<int>("foo"));
            mocks.VerifyAll();
        }

		[Test]
		public void CanMockAppDomain()
		{
			MockRepository mocks = new MockRepository();
			AppDomain appDomain = mocks.CreateMock<AppDomain>();
			Expect.Call(appDomain.BaseDirectory).Return("/home/user/ayende");
			mocks.ReplayAll();
			Assert.AreEqual(appDomain.BaseDirectory, "/home/user/ayende" );
			mocks.VerifyAll();
		}

		[Test, ExpectedException(typeof(ExpectationViolationException),
@"AppDomain.get_BaseDirectory(); Expected #1, Actual #0.")]
    	public void NotCallingExpectedMethodWillCauseVerificationError()
    	{
			MockRepository mocks = new MockRepository();
			AppDomain appDomain = mocks.CreateMock<AppDomain>();
			Expect.Call(appDomain.BaseDirectory).Return("/home/user/ayende");
			mocks.ReplayAll();
			mocks.VerifyAll();
    	}
    }
}
