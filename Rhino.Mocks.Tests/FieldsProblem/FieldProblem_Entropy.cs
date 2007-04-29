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
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Entropy
	{
		public interface IMyObject
		{
			void DoSomething();
			void DoSomethingElse();
			object SomeProperty { get; set; }
		}

		[Test]
		public void NestedOrderedAndAtLeastOnce()
		{
			MockRepository mocks = new MockRepository();
			IMyObject myObject = mocks.CreateMock(typeof(IMyObject)) as IMyObject;

			using (mocks.Ordered())
			{
				using (mocks.Ordered()) // <-- Works if removed. Unordered does not work too.
				{
					Expect.Call(myObject.SomeProperty).Return(null).Repeat.AtLeastOnce();
				}
				myObject.DoSomething();
			}

			mocks.ReplayAll();

			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			myObject.DoSomething();

			mocks.VerifyAll();
		}


		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IMyObject.DoSomething(); Expected #1, Actual #0.")]
		public void ShouldFailInNestedOrderringIfMethodWasNotCalled()
		{
			MockRepository mocks = new MockRepository();
			IMyObject myObject = mocks.CreateMock(typeof(IMyObject)) as IMyObject;

			using (mocks.Ordered())
			{
				using (mocks.Ordered()) 
				{
					myObject.DoSomethingElse();
					LastCall.Repeat.AtLeastOnce();
				}
				myObject.DoSomething(); 
			}

			mocks.ReplayAll();
			myObject.DoSomethingElse(); 
			mocks.VerifyAll();
		}

		[Test]
		public void NestedInorderedAndAtLeastOnce()
		{
			MockRepository mocks = new MockRepository();
			IMyObject myObject = mocks.CreateMock(typeof(IMyObject)) as IMyObject;

			using (mocks.Ordered())
			{
				using (mocks.Unordered()) // <-- Works only if ordered
				{
					Expect.Call(myObject.SomeProperty).Return(null).Repeat.AtLeastOnce();
				}
				myObject.DoSomething();
			}

			mocks.ReplayAll();

			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			myObject.DoSomething();
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), @"IMyObject.DoSomething(); Expected #0, Actual #1.")]
		public void UnorderedAndAtLeastOnce_CallingAnExtraMethod()
		{
			MockRepository mocks = new MockRepository();
			IMyObject myObject = mocks.CreateMock(typeof(IMyObject)) as IMyObject;

			using (mocks.Unordered())
			{
				Expect.Call(myObject.SomeProperty).Return(null).Repeat.AtLeastOnce();
			}
			myObject.DoSomethingElse();

			mocks.ReplayAll();

			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			myObject.DoSomething();

			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), @"IMyObject.DoSomething(); Expected #0, Actual #1.")]
		public void OrderedAndAtLeastOnce_CallingAnExtraMethod()
		{
			MockRepository mocks = new MockRepository();
			IMyObject myObject = mocks.CreateMock(typeof(IMyObject)) as IMyObject;

			using (mocks.Ordered())
			{
				Expect.Call(myObject.SomeProperty).Return(null).Repeat.AtLeastOnce();
			}
			myObject.DoSomethingElse();

			mocks.ReplayAll();

			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			myObject.DoSomething();

			mocks.VerifyAll();
		}

	}
}
