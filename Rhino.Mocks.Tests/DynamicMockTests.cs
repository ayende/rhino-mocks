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
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class DynamicMockTests
	{
		MockRepository mocks;
		IDemo demo;
		private bool doNotVerifyOnTearDown ;

		[SetUp]
		public void Setup()
		{
			doNotVerifyOnTearDown = false;
			mocks = new MockRepository();
			demo = (IDemo)mocks.DynamicMock(typeof(IDemo));
		}

		[TearDown]
		public void Teardown()
		{
			if(doNotVerifyOnTearDown)
			mocks.VerifyAll();
		}

		[Test]
		public void CanCallUnexpectedMethodOnDynamicMock()
		{
			mocks.ReplayAll();
			Assert.AreEqual(0,demo.ReturnIntNoArgs());
		}

		[Test]
		public void CanSetupExpectations()
		{
			Expect.Call(demo.ReturnIntNoArgs()).Return(30).Repeat.Once();
			mocks.ReplayAll();
			Assert.AreEqual(30,demo.ReturnIntNoArgs(),"Expected call didn't return setup value");
			Assert.AreEqual(0,demo.ReturnIntNoArgs(),"Unexpected call return non default value");
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException),"IDemo.ReturnIntNoArgs(); Expected #1, Actual #0." )]
		public void ExpectationExceptionWithDynamicMock()
		{
			Expect.Call(demo.ReturnIntNoArgs()).Return(30);
			mocks.ReplayAll();
			Assert.IsNull(demo.ReturnStringNoArgs());
			doNotVerifyOnTearDown = true;
			mocks.Verify(demo);	
		}

		[Test]
		public void SetupResultWorksWithDynamicMocks()
		{
			SetupResult.For(demo.StringArgString("Ayende")).Return("Rahien");
			mocks.ReplayAll();
			for (int i = 0; i < 43; i++)
			{
				Assert.AreEqual("Rahien",demo.StringArgString("Ayende"));
				Assert.IsNull(demo.StringArgString("another"));
			}
		}

		[Test]
		public void ExpectNeverForDyanmicMock()
		{
			Expect.Call(demo.ReturnIntNoArgs()).Repeat.Never();
			mocks.ReplayAll();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException),"IDemo.ReturnIntNoArgs(); Expected #0, Actual #1.")]
		public void ExpectNeverForDyanmicMockThrowsIfOccurs()
		{
			Expect.Call(demo.ReturnIntNoArgs()).Repeat.Never();
			mocks.ReplayAll();
			demo.ReturnIntNoArgs();
		}
	}
}
