﻿#region license
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
namespace Rhino.Mocks.Tests.FieldsProblem
{
	using System;
	using Xunit;
	using Rhino.Mocks.Constraints;

	
	public class FieldProblem_Andri
	{
		public class AndriTest
		{
			/// <summary>
			/// The value of a variable used as a ref parameter should be used as a constraint on an expectation
			/// even when if it is marked with an InteropServices.OutAttribute
			/// </summary>
			[Fact]
			public void OutByRefTest()
			{
				MockRepository mockery = new MockRepository();
				IFoo mockFoo = mockery.StrictMock<IFoo>();
				int three = 3;
				int six = 6;
				using (mockery.Record())
				{
					SetupResult.For(mockFoo.foo(ref three)).OutRef(six).Return(true);
				}

				Assert.Throws<Rhino.Mocks.Exceptions.ExpectationViolationException>(() => mockFoo.foo(ref six));
			}
		}

		#region Nested type: IFoo

		public interface IFoo
		{
			bool foo([System.Runtime.InteropServices.Out] [System.Runtime.InteropServices.In] ref int fooSquared);
		}

		#endregion
	}
}
