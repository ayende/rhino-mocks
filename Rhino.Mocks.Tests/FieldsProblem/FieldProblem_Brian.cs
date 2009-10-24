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
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	
	public class FieldProblem_Brian
	{
		[Fact]
		public void SetExpectationOnNullableValue()
		{
			MockRepository mocks = new MockRepository();
			IFoo foo = mocks.StrictMock<IFoo>();

			int? id = 2;

			Expect.Call(foo.Id).Return(id).Repeat.Twice();
			Expect.Call(foo.Id).Return(null);
			Expect.Call(foo.Id).Return(1);

			mocks.ReplayAll();

			Assert.True(foo.Id.HasValue);
			Assert.Equal(2, foo.Id.Value);
			Assert.False(foo.Id.HasValue);
			Assert.Equal(1, foo.Id.Value);

			mocks.VerifyAll();
		}

		[Fact]
		public void MockingInternalMetohdsAndPropertiesOfInternalClass()
		{
			
			TestClass testClass = new TestClass();

			string testMethod = testClass.TestMethod();
			string testProperty = testClass.TestProperty;

			MockRepository mockRepository = new MockRepository();

			TestClass mockTestClass = mockRepository.StrictMock<TestClass>();

			Expect.Call(mockTestClass.TestMethod()).Return("MockTestMethod");
			Expect.Call(mockTestClass.TestProperty).Return("MockTestProperty");

			mockRepository.ReplayAll();

			Assert.Equal("MockTestMethod", mockTestClass.TestMethod());
			Assert.Equal("MockTestProperty", mockTestClass.TestProperty);

			mockRepository.VerifyAll();

		}

		public interface IFoo
		{
			int? Id { get;}
		}

		internal class TestClass
		{
			internal virtual string TestMethod()
			{
				return "TestMethod";
			}

			internal virtual string TestProperty
			{
				get { return "TestProperty"; }
			}
		}
	}
}
