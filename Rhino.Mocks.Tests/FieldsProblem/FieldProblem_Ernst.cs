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
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Ernst
	{
		[Test]
		public void CallOriginalMethodProblem2()
		{
			MockRepository mockRepository = new MockRepository();
			MockedClass mock = mockRepository.CreateMock<MockedClass>();

			mock.Method(null);
			LastCall.Constraints(Is.Equal("parameter")).CallOriginalMethod
				(OriginalCallOptions.CreateExpectation);

			mockRepository.ReplayAll();

			mock.Method("parameter");

			mockRepository.VerifyAll();
		}

		[Test]
		public void CanUseBackToRecordOnMethodsThatCallToCallOriginalMethod()
		{
			MockRepository repository = new MockRepository();
			TestClass mock = repository.CreateMock<TestClass>();

			mock.Method();
			LastCall.CallOriginalMethod
				(OriginalCallOptions.NoExpectation);

			repository.ReplayAll();
			mock.Method();
			repository.VerifyAll();

			repository.BackToRecordAll();

			mock.Method();
			LastCall.Throw(new ApplicationException());

			repository.ReplayAll();

			try
			{
				mock.Method();
				Assert.Fail();
			}
			catch (ApplicationException ex)
			{
			}
			repository.VerifyAll();
		}


		[Test]
		public void CanUseBackToRecordOnMethodsThatCallPropertyBehavior()
		{
			MockRepository repository = new MockRepository();
			TestClass mock = repository.CreateMock<TestClass>();

			Expect.Call(mock.Id).PropertyBehavior();

			repository.ReplayAll();
			mock.Id = 4;
			int d = mock.Id;
			Assert.AreEqual(4,d );
			repository.VerifyAll();

			repository.BackToRecordAll();

			Expect.Call(mock.Id).Return(5);

			repository.ReplayAll();

			Assert.AreEqual(5, mock.Id);

			repository.VerifyAll();
		}
	}

	public class TestClass
	{
		private int id;


		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual void Method()
		{
		}
	}

	public class MockedClass
	{
		public virtual void Method(string parameter)
		{
			if (parameter == null)
				throw new ArgumentNullException();

			//Something in this method must be executed
		}
	}
}