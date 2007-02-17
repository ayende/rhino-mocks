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
			IExpectation expectation = new ConstraintsExpectation(new FakeInvocation(m),new AbstractConstraint[0]); 
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
				});
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
			});
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: constraints")]
		public void NullConstraints()
		{
			new ConstraintsExpectation(new FakeInvocation(this.method), null);
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
				});
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
				});

		}

		[Test]
		public void CreateErrorMessageForConstraints()
		{
			ConstraintsExpectation expectation = new ConstraintsExpectation(new FakeInvocation(this.method), new AbstractConstraint[]
				{
					Is.Anything(),
					Is.Null(),
					Text.Like(@"[\w\d]+1234")
				});
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