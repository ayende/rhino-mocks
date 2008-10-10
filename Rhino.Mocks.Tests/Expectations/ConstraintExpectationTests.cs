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
using System.Reflection;
using MbUnit.Framework;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.Expectations
{
	[TestFixture]
	public class ConstraintExpectationTests:AbstractExpectationTests
	{
		private MethodInfo method;
		private ConstraintsExpectation expectation;

		protected override IExpectation GetExpectation(MethodInfo m, Range r, int actual)
		{
			IExpectation expectation = new ConstraintsExpectation(new FakeInvocation(m),new AbstractConstraint[0], new Range(1, 1)); 
			SetupExpectation(expectation, r, actual);
			return expectation;
		}

		[SetUp]
		public void SetUp()
		{
			method = typeof (IDemo).GetMethod("VoidThreeArgs");
			expectation = new ConstraintsExpectation(new FakeInvocation(this.method), new AbstractConstraint[]
				{
					Is.Anything(),
					Text.Like(@"[\w\d]+"),
					Is.Equal(3.14f),
				}, new Range(1, 1));
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "The constraint at index 1 is null! Use Is.Null() to represent null parameters.")]
		public void PassingNullConstraintsThrows()
		{
			expectation = new ConstraintsExpectation(new FakeInvocation(this.method), new AbstractConstraint[]
				{
					Is.Anything(),
					null,
					Is.Equal(3.14f),
			}, new Range(1, 1));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: constraints")]
		public void NullConstraints()
		{
			new ConstraintsExpectation(new FakeInvocation(this.method), null, new Range(1, 1));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: constraints")]
		public void NullConstraintsFromPrevExpectation()
		{
			new ConstraintsExpectation(expectation, null);
		}


		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: args")]
		public void NullEvaluation()
		{
			expectation.IsExpected(null);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "The number of constraints is not the same as the number of the method's parameters!")]
		public void TooFewConstraints()
		{
			new ConstraintsExpectation(new FakeInvocation(this.method), new AbstractConstraint[]
				{
					Is.Anything(),
					Is.Null()
				}, new Range(1, 1));
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "The number of constraints is not the same as the number of the method's parameters!")]
		public void TooManyConstraints()
		{
			new ConstraintsExpectation(new FakeInvocation(this.method), new AbstractConstraint[]
				{
					Is.Anything(),
					Is.Null(),
					Is.NotNull(),
					Text.Like("Song")
				}, new Range(1, 1));

		}

		[Test]
		public void CreateErrorMessageForConstraints()
		{
			ConstraintsExpectation expectation = new ConstraintsExpectation(new FakeInvocation(this.method), new AbstractConstraint[]
				{
					Is.Anything(),
					Is.Null(),
					Text.Like(@"[\w\d]+1234")
				}, new Range(1, 1));
			string message = "IDemo.VoidThreeArgs(anything, equal to null, like \"[\\w\\d]+1234\");";
			Assert.AreEqual(message, expectation.ErrorMessage);
		}

		

		[Test]
		public void ValidateConstraint()
		{
			Assert.IsTrue(this.expectation.IsExpected(new object[] {32, "Ayende", 3.14f}));
		}

		[Test]
		public void FailToValidateConstraint()
		{
			Assert.IsFalse(this.expectation.IsExpected(new object[] {32, "Ayende", 4.13f}));
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Number of argument doesn't match the number of parameters!")]
		public void TooFewArgs()
		{
			this.expectation.IsExpected(new object[] {32, "Ayende"});
		}
	}
}