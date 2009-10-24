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
using System.Windows.Forms;
using Xunit;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
    
    public class PartialMockTests
    {
        MockRepository mocks;
        AbstractClass abs;

		public PartialMockTests()
        {
            mocks = new MockRepository();
            abs = (AbstractClass)mocks.PartialMock(typeof(AbstractClass));
        }

        [Fact]
		public void AutomaticallCallBaseMethodIfNoExpectationWasSet() 
	    {
            mocks.ReplayAll();
            Assert.Equal(1, abs.Increment());
			Assert.Equal(6, abs.Add(5));
			Assert.Equal(6, abs.Count);
			mocks.VerifyAll();

	    }

        [Fact]
        public void CanMockVirtualMethods()
        {
            Expect.Call(abs.Increment()).Return(5);
			Expect.Call(abs.Add(2)).Return(3);
			mocks.ReplayAll();
            Assert.Equal(5, abs.Increment());
			Assert.Equal(3, abs.Add(2));
			Assert.Equal(0, abs.Count);
            mocks.VerifyAll();
        }

        [Fact]
        public void CanMockAbstractMethods()
        {
            Expect.Call(abs.Decrement()).Return(5);
            mocks.ReplayAll();
            Assert.Equal(5, abs.Decrement());
            Assert.Equal(0, abs.Count);
            mocks.VerifyAll();
        }

        [Fact]
        public void CantCreatePartialMockFromInterfaces()
        {
        	Assert.Throws<InvalidOperationException>("Can't create a partial mock from an interface",
        	                                         () => new MockRepository().PartialMock(typeof (IDemo)));
        }

        [Fact]
        public void CallAnAbstractMethodWithoutSettingExpectation()
        {
            mocks.ReplayAll();
			Assert.Throws<ExpectationViolationException>("AbstractClass.Decrement(); Expected #0, Actual #1.",
												 () => abs.Decrement());
			;
        }

    	[Fact]
    	public void CanMockWithCtorParams()
    	{
    		WithParameters withParameters = mocks.PartialMock<WithParameters>(1);
    		Expect.Call(withParameters.Int).Return(4);
    		mocks.ReplayAll();
    		Assert.Equal(4, withParameters.Int);
    		mocks.VerifyAll();
    	}
    }
    
    public abstract class AbstractClass
    {
        public int Count = 0;

        public virtual int Increment()
        {
            return ++Count;
        }

        public virtual int Add(int n)
		{
			return Count += n;
		}

        public abstract int Decrement();
    }

	public class WithParameters
	{
		private int i;


		public WithParameters(int i)
		{
			this.i = i;
		}


		public virtual int Int
		{
			get { return i; }
			set { i = value; }
		}
	}
}
