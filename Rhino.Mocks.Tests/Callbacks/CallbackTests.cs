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
using System.Runtime.InteropServices;
using Xunit;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests.Callbacks
{
	
	public class CallbackTests
	{
		private MockRepository mocks;
		private IDemo demo;
		private bool callbackCalled;

		public CallbackTests()
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			mocks = new MockRepository();
			demo = (IDemo) mocks.StrictMock(typeof (IDemo));
			callbackCalled = false;
		}

		[Fact]
		public void CallbackIsCalled()
		{
			demo.VoidStringArg("Ayende");
			LastCall.On(demo).Callback<string>(StringMethod);
			mocks.Replay(demo);
			demo.VoidStringArg("");
			mocks.Verify(demo);
			Assert.True(callbackCalled);
		}

		[Fact]
		public void GetSameArgumentsAsMethod()
		{
			demo.VoidThreeArgs(0, "", 0f);
			LastCall.On(demo).Callback<int, string, float>(ThreeArgsAreSame);
			mocks.Replay(demo);
			demo.VoidThreeArgs(1, "Ayende", 3.14f);
			mocks.Verify(demo);
			Assert.True(callbackCalled);
		}

		[Fact]
		public void DifferentArgumentsFromMethodThrows()
		{
			demo.VoidThreeArgs(0, "", 0f);
			Assert.Throws<InvalidOperationException>("Callback arguments didn't match the method arguments",
			                                         ()=> LastCall.On(demo).Callback<int, string, string>(OtherThreeArgs));
		}

		[Fact]
		public void IgnoreArgsWhenUsingCallbacks()
		{
			demo.VoidThreeArgs(0, "", 0f);
			LastCall.On(demo).Callback<int, string, float>(ThreeArgsAreSame);
			mocks.Replay(demo);
			demo.VoidThreeArgs(1, "Ayende", 3.14f);
			mocks.Verify(demo);
		}

		[Fact]
		public void SetReturnValueOnMethodWithCallback()
		{
			demo.ReturnIntNoArgs();
			LastCall.On(demo).Callback(NoArgsMethod).Return(5);
			mocks.Replay(demo);
			Assert.Equal(5, demo.ReturnIntNoArgs());
			mocks.Verify(demo);
		}

		[Fact]
		public void CallbackWithDifferentSignatureFails()
		{
			demo.VoidThreeArgs(0, "", 0f);
			Assert.Throws<InvalidOperationException>(
				"Callback arguments didn't match the method arguments",
				() => LastCall.On(demo).Callback<string>(StringMethod));
		}

		[Fact]
		public void GetMessageFromCallbackWhenNotReplaying()
		{
			demo.VoidThreeArgs(0, "", 0f);
			LastCall.On(demo).Callback<int, string, float>(ThreeArgsAreSame);
			mocks.Replay(demo);
			Assert.Throws<ExpectationViolationException>(
				"IDemo.VoidThreeArgs(callback method: CallbackTests.ThreeArgsAreSame); Expected #1, Actual #0.",
				() => mocks.Verify(demo));
		}

		[Fact]
		public void GetMessageFromCallbackWhenCalledTooMuch()
		{
			demo.VoidThreeArgs(0, "", 0f);
			LastCall.On(demo).Callback<int, string, float>(ThreeArgsAreSame);
			mocks.Replay(demo);
			demo.VoidThreeArgs(1, "Ayende", 3.14f);
			
			Assert.Throws<ExpectationViolationException>("IDemo.VoidThreeArgs(1, \"Ayende\", 3.14); Expected #1, Actual #2.",
														 () => demo.VoidThreeArgs(1, "Ayende", 3.14f));
		}


		[Fact]
		public void CallbackWhenMethodHasReturnValue()
		{
			demo.ReturnIntNoArgs();
			LastCall.On(demo).Callback(NoArgsMethod);
			Assert.Throws<InvalidOperationException>(
				"Previous method 'IDemo.ReturnIntNoArgs(callback method: CallbackTests.NoArgsMethod);' requires a return value or an exception to throw.",
				() => mocks.Replay(demo));
		}


		[Fact]
		public void CallbackAndConstraintsOnSameMethod()
		{
			demo.StringArgString("");
			Assert.Throws<InvalidOperationException>("This method has already been set to CallbackExpectation.",
			                                         () => LastCall.On(demo).Callback<string>(StringMethod)
			                                               	.Constraints(Is.Anything()));
		}

		[Fact]
		public void ExceptionInCallback()
		{
			demo.ReturnIntNoArgs();
			LastCall.On(demo).Callback(NoArgsThrowing).Return(5);
			mocks.Replay(demo);
			Assert.Throws<ExternalException>("I'm not guilty, is was /him/",
			                                 () => Assert.Equal(5, demo.ReturnIntNoArgs()));
		}

		[Fact]
		public void CallbackCanFailExpectationByReturningFalse()
		{
			demo.VoidNoArgs();
			LastCall.On(demo).Callback(NoArgsMethodFalse);
			mocks.Replay(demo);
			Assert.Throws<ExpectationViolationException>("IDemo.VoidThreeArgs(1, \"Ayende\", 3.14); Expected #0, Actual #1.",
			                                             () => demo.VoidThreeArgs(1, "Ayende", 3.14f));
		}

		#region Implementation Details

		private bool StringMethod(string s)
		{
			callbackCalled = true;
			return true;
		}

		private bool OtherThreeArgs(int i, string s, string s2)
		{
			return true;
		}

		private bool ThreeArgsAreSame(int i, string s, float f)
		{
			Assert.Equal(1, i);
			Assert.Equal("Ayende", s);
			Assert.Equal(3.14f, f);
			callbackCalled = true;
			return true;
		}

		private bool NoArgsMethod()
		{
			return true;
		}

		private bool NoArgsMethodFalse()
		{
			return false;
		}

		private bool NoArgsThrowing()
		{
			throw new ExternalException("I'm not guilty, is was /him/");
		}

		#endregion
	}

	public class DelegateDefinations
	{
		public delegate void VoidThreeArgsDelegate(int i, string s, float f);

		public delegate bool StringDelegate(string s);

		public delegate bool ThreeArgsDelegate(int i, string s, float f);
		public delegate bool OtherThreeArgsDelegate(int i, string s, string s2);

		public delegate bool NoArgsDelegate();
		public delegate bool IntArgDelegate(int i);

	}
}
