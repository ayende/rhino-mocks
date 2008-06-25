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

#if DOTNET35

using MbUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Jodran
	{
		[Test]
		public void CanUseExpectSyntax_OnStubWithOrderedExpectations()
		{
			MockRepository mocks = new MockRepository();

			var foo54 = mocks.Stub<IFoo54>();


			using (mocks.Ordered())
			{
				foo54
					.Expect(x => x.DoSomething())
					.Return(0);

				foo54
					.Expect(x => x.DoSomethingElse());
			}

			mocks.Replay(foo54);
			
			foo54.DoSomething();
			foo54.DoSomethingElse();

			foo54.VerifyAllExpectations();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException))]
		public void CanUseExpectSyntax_OnStubWithOrderedExpectations2()
		{
			MockRepository mocks = new MockRepository();

			var foo54 = mocks.Stub<IFoo54>();


			using (mocks.Ordered())
			{
				foo54
					.Expect(x => x.DoSomething())
					.Return(0);

				foo54
					.Expect(x => x.DoSomethingElse());
			}
			
			mocks.Replay(foo54);

			foo54.DoSomethingElse();
			foo54.DoSomething();

			foo54.VerifyAllExpectations();
		}

		[Test]
		public void CanUseExpectSyntax_OnMockbWithOrderedExpectations()
		{
			MockRepository mocks = new MockRepository();

			var foo54 = mocks.StrictMock<IFoo54>();


			using (mocks.Ordered())
			{
				foo54
					.Expect(x => x.DoSomething())
					.Return(0);

				foo54
					.Expect(x => x.DoSomethingElse());
			}

			mocks.Replay(foo54);
			
			foo54.DoSomething();
			foo54.DoSomethingElse();

			foo54.VerifyAllExpectations();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException))]
		public void CanUseExpectSyntax_OnMockWithOrderedExpectations2()
		{
			MockRepository mocks = new MockRepository();

			var foo54 = mocks.StrictMock<IFoo54>();


			using (mocks.Ordered())
			{
				foo54
					.Expect(x => x.DoSomething())
					.Return(0);

				foo54
					.Expect(x => x.DoSomethingElse());
			}

			mocks.Replay(foo54);

			foo54.DoSomethingElse();
			foo54.DoSomething();

			foo54.VerifyAllExpectations();
		}

	}
}
#endif