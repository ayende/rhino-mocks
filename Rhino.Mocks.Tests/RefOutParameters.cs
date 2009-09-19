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
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class RefOutParameters
    {
        public class MyClass
        {
            public virtual void MyMethod(out int i, ref string s, int i1, out string s2)
            {
                throw new NotImplementedException(); 
            }
        }

        [Test]
        public void UseTheOutMethodToSpecifyOutputAndRefParameters()
        {
            MockRepository mocks = new MockRepository();
            MyClass myClass = (MyClass) mocks.StrictMock(typeof (MyClass));
            int i;
            string s = null, s2;
            myClass.MyMethod(out i, ref s, 1, out s2);
            LastCall.IgnoreArguments().OutRef(100, "s", "b");
            mocks.ReplayAll();
            
            myClass.MyMethod(out i, ref s, 1, out s2);
            
            mocks.VerifyAll();
            
            Assert.AreEqual(100, i);
            Assert.AreEqual("s", s);
            Assert.AreEqual("b", s2);
        }

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Output and ref parameters has already been set for this expectation")]
        public void UseTheOutMethodToSpecifyOutputAndRefParameters_CanOnlyBeCalledOnce()
        {
            MockRepository mocks = new MockRepository();
            MyClass myClass = (MyClass) mocks.StrictMock(typeof (MyClass));
            int i;
            string s = null, s2;
            myClass.MyMethod(out i, ref s, 1, out s2);
			LastCall.OutRef(100, "s", "b").OutRef(100, "s", "b");
        }

    	[Test]
    	public void GivingLessParametersThanWhatIsInTheMethodWillNotThrow()
    	{
    		   MockRepository mocks = new MockRepository();
            MyClass myClass = (MyClass) mocks.StrictMock(typeof (MyClass));
            int i;
            string s = null, s2;
            myClass.MyMethod(out i, ref s, 1, out s2);
            LastCall.IgnoreArguments().OutRef(100);
            mocks.ReplayAll();
            
            myClass.MyMethod(out i, ref s, 1, out s2);
            
            mocks.VerifyAll();
            
            Assert.AreEqual(100, i);
            Assert.IsNull(s);
            Assert.IsNull(s2);
    	}
    }
}
