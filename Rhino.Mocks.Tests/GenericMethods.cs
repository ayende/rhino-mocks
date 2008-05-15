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
	public class GenericMethods
	{
		[Test]
		public void CanStrictMockOfInterfaceWithGenericMethod()
		{
			MockRepository mocks = new MockRepository();
			mocks.StrictMock<IFactory>();
		}

		[Test]
		public void CanSetExpectationsOnInterfaceWithGenericMethod()
		{
			MockRepository mocks = new MockRepository();
			IFactory factory = mocks.StrictMock<IFactory>();
			Expect.Call(factory.Create<string>()).Return("working?");
			mocks.ReplayAll();
			string result = factory.Create<string>();
			Assert.AreEqual("working?",result, "Should have worked, hm..." );
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException),@"IFactory.Create<System.Int32>(); Expected #1, Actual #1.
IFactory.Create<System.String>(); Expected #1, Actual #0." )]
		public void WillGetErrorIfCallingMethodWithDifferentGenericArgument()
		{
			MockRepository mocks = new MockRepository();
			IFactory factory = mocks.StrictMock<IFactory>();
			Expect.Call(factory.Create<string>()).Return("working?");
			mocks.ReplayAll();
			factory.Create<int>();
		}

//		Won't compile anymore
//		[Test]
//		[ExpectedException(typeof(InvalidOperationException),"Type 'System.Int32' doesn't match the return type 'System.String' for method 'IFactory.Create<System.String>();'")]
//		public void WillGiveErrorIfThereIsTypeMismatchInGenericParameters()
//		{
//			MockRepository mocks = new MockRepository();
//			IFactory factory = mocks.StrictMock<IFactory>();
//			Expect.Call(factory.Create<string>()).Return(1);
//		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IFactory.Create<System.String>(); Expected #1, Actual #0.")]
		public void WillGiveErrorIfMissingCallToGenericMethod()
		{
			MockRepository mocks = new MockRepository();
			IFactory factory = mocks.StrictMock<IFactory>();
			Expect.Call(factory.Create<string>()).Return("working?");
			mocks.ReplayAll();
			mocks.VerifyAll();
		}

	}



	public interface IFactory
	{
		T Create<T>();
	}
}