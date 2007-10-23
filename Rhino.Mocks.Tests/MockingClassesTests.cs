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
using MbUnit.Framework;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class MockingClassesTests
	{
		private MockRepository mocks;
		private DemoClass demoClass;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			demoClass = (DemoClass) mocks.CreateMock(typeof (DemoClass));
		}


		[Test]
		public void MockAClass()
		{
			IMockedObject mockedObject = demoClass as IMockedObject;
			Assert.AreEqual(mocks, mockedObject.Repository);
		}

		[Test]
		public void MockVirtualCall()
		{
			demoClass.Two();
			LastCall.On(demoClass).Return(3);
			mocks.Replay(demoClass);
			Assert.AreEqual(3, demoClass.Two());
			mocks.Verify(demoClass);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "There is no matching last call on this object. Are you sure that the last call was a virtual or interface method call?")]
		public void CantMockNonVirtualCall()
		{
			demoClass.One();
			LastCall.On(demoClass).Return(3);

		}

		[Test]
		public void MockClassWithParametrizedCtor()
		{
			ParametrizedCtor pc = mocks.CreateMock(typeof (ParametrizedCtor), 3, "Hello") as ParametrizedCtor;
			Assert.AreEqual(3, pc.Int);
			Assert.AreEqual("Hello", pc.String);
			pc.Add(0, 1);
			LastCall.On(pc).Return(10);
			mocks.Replay(pc);
			Assert.AreEqual(10, pc.Add(0, 1));
			mocks.Verify(pc);
		}

		[Test]
		public void MockClassWithOverloadedCtor()
		{
			OverLoadedCtor oc = mocks.CreateMock(typeof (OverLoadedCtor), 1) as OverLoadedCtor;
			OverLoadCtorExercise(oc, 1, null);
			oc = mocks.CreateMock(typeof (OverLoadedCtor), "Hello") as OverLoadedCtor;
			OverLoadCtorExercise(oc, 0, "Hello");
			oc = mocks.CreateMock(typeof (OverLoadedCtor), 33, "Hello") as OverLoadedCtor;
			OverLoadCtorExercise(oc, 33, "Hello");
		}

		[Test]
		[ExpectedException(typeof (MissingMethodException), "Can't find a constructor with matching arguments")]
		public void BadParamsToCtor()
		{
			mocks.CreateMock(typeof (OverLoadedCtor), "Ayende", 55);
		}


		[Test]
		[ExpectedException(typeof (NotSupportedException), "Can't create mocks of sealed classes")]
		public void MockSealedClass()
		{
			MockRepository mocks = new MockRepository();
			mocks.CreateMock(typeof (File));
		}

        [Test]
        [ExpectedException(typeof (InvalidOperationException),"Invalid call, the last call has been used or no call has been made (make sure that you are calling a virtual (C#) / Overridable (VB) method).")]
        public void CallNonVirtualMethodThatImplementsAnInterface()
        {
            ((IDisposable)demoClass).Dispose();
            LastCall.Repeat.Never();
           
        }
    

		#region Can call object's method without implementing them

		[Test]
		public void ToStringMocked()
		{
            if (demoClass.ToString()=="")
            {
                Assert.Fail("ToString() of a mocked object is empty");
            }
		}

		[Test]
		public void GetTypeMocked()
		{
			Assert.IsTrue(typeof (DemoClass).IsAssignableFrom(demoClass.GetType()));
		}

		[Test]
		public void GetHashCodeMocked()
		{
			Assert.AreEqual(demoClass.GetHashCode(), demoClass.GetHashCode(), "Hash code doesn't remain the same across invocations");
		}

		[Test]
		public void EqualsMocked()
		{
			Assert.IsTrue(demoClass.Equals(demoClass));
		}

		#endregion

		#region Classes 

		public class DemoClass : IDisposable
		{
			int _prop;
		    public bool disposableCalled;

		    public virtual int Prop
			{
				get { return _prop; }
				set { _prop = value; }
			}

			public int One()
			{
				return 1;
			}

			public virtual int Two()
			{
				return 2;
			}

             void IDisposable.Dispose()
		    {
                disposableCalled = true;
		    }
		}

		public abstract class AbstractDemo
		{
			public virtual int Five()
			{
				return 0;
			}

			public abstract string Six();
		}

		public class ParametrizedCtor
		{
			private int i;
			private string s;

			public ParametrizedCtor(int i, string s)
			{
				this.i = i;
				this.s = s;
			}

			public int Int
			{
				get { return i; }
				set { i = value; }
			}

			public string String
			{
				get { return s; }
				set { s = value; }
			}

			public virtual int Add(int i1, int i2)
			{
				return i1 + i2;
			}

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
		}

		public class OverLoadedCtor
		{
			private int i;
			private string s;

			public OverLoadedCtor(int i)
			{
				this.i = i;
			}

			public OverLoadedCtor(string s)
			{
				this.s = s;
			}

			public OverLoadedCtor(int i, string s)
			{
				this.i = i;
				this.s = s;
			}

			public int I
			{
				get { return i; }
			}

			public string S
			{
				get { return s; }
			}

			public virtual string Concat(string s1, string s2)
			{
				return s1 + s2;
			}

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
		}

		#endregion

		#region Implementation

		private void OverLoadCtorExercise(OverLoadedCtor oc, int i, string s)
		{
			Assert.AreEqual(i, oc.I);
			Assert.AreEqual(s, oc.S);
			oc.Concat("Ayende", "Rahien");
			LastCall.On(oc).Return("Hello, World");
			mocks.Replay(oc);
			Assert.AreEqual("Hello, World", oc.Concat("Ayende", "Rahien"));
			mocks.Verify(oc);
		}

		#endregion
	}
}