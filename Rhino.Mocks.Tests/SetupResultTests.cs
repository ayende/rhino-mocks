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
using Xunit;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
	
	public class SetupResultTests
	{
		private MockRepository mocks;
		private IDemo demo;

		public SetupResultTests()
		{
			mocks = new MockRepository();
			demo = mocks.StrictMock(typeof (IDemo)) as IDemo;
		}

        [Fact]
        public void CanSetupResultForMethodAndIgnoreArgs()
        {
            SetupResult.For(demo.StringArgString(null)).Return("Ayende").IgnoreArguments();
            mocks.ReplayAll();
            Assert.Equal("Ayende", demo.StringArgString("a"));
            Assert.Equal("Ayende", demo.StringArgString("b"));
            mocks.VerifyAll();
            
        }
	    
		[Fact]
		public void CanSetupResult()
		{
			SetupResult.For(demo.Prop).Return("Ayende");
			mocks.ReplayAll();
			Assert.Equal("Ayende", demo.Prop);
            mocks.VerifyAll();
		    
		}

		[Fact]
		public void SetupResultForNoCall()
		{
			Assert.Throws<InvalidOperationException>(
				"Invalid call, the last call has been used or no call has been made (make sure that you are calling a virtual (C#) / Overridable (VB) method).",
				() => SetupResult.For<object>(null));
		}

		[Fact]
		public void SetupResultCanRepeatAsManyTimeAsItWant()
		{
			SetupResult.For(demo.Prop).Return("Ayende");
			mocks.ReplayAll();
			for (int i = 0; i < 30; i++)
			{
				Assert.Equal("Ayende", demo.Prop);
			}
            mocks.VerifyAll();
		    
		}

		[Fact]
		public void SetupResultUsingOn()
		{
			SetupResult.On(demo).Call(demo.Prop).Return("Ayende");
			mocks.ReplayAll();
			for (int i = 0; i < 30; i++)
			{
				Assert.Equal("Ayende", demo.Prop);
			}
            mocks.VerifyAll();
		    
		}

		[Fact]
		public void SetupResultUsingOrdered()
		{
			SetupResult.On(demo).Call(demo.Prop).Return("Ayende");
			using (mocks.Ordered())
			{
				demo.VoidNoArgs();
				LastCall.On(demo).Repeat.Twice();
			}
			mocks.ReplayAll();
			demo.VoidNoArgs();
			for (int i = 0; i < 30; i++)
			{
				Assert.Equal("Ayende", demo.Prop);
			}
			demo.VoidNoArgs();
            mocks.VerifyAll();
		    
		}

		[Fact]
		public void SetupResultForTheSameMethodTwiceCauseExcetion()
		{
			SetupResult.On(demo).Call(demo.Prop).Return("Ayende");
			Assert.Throws<InvalidOperationException>( "The result for IDemo.get_Prop(); has already been setup.", () => SetupResult.On(demo).Call(demo.Prop).Return("Ayende"));
		}

		[Fact]
		public void ExpectNever()
		{
			demo.ReturnStringNoArgs();
			LastCall.Repeat.Never();
			mocks.ReplayAll();
			Assert.Throws<ExpectationViolationException>("IDemo.ReturnIntNoArgs(); Expected #0, Actual #1.", () => demo.ReturnIntNoArgs());
		}

		[Fact]
		public void ExpectNeverSetupTwiceThrows()
		{
			demo.ReturnStringNoArgs();
			LastCall.Repeat.Never();
			demo.ReturnStringNoArgs();
			Assert.Throws<InvalidOperationException>("The result for IDemo.ReturnStringNoArgs(); has already been setup.", () => LastCall.Repeat.Never());
			
		}
	}
}