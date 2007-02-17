using System;
using System.Reflection;
using MbUnit.Framework;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.Expectations
{
	[TestFixture]
	public class ArgsEqualTests : AbstractExpectationTests
	{
		private readonly MethodInfo method = typeof (IDemo).GetMethod("VoidThreeArgs");

		protected override IExpectation GetExpectation(MethodInfo m, Range r, int actual)
		{
			ArgsEqualExpectation expectation = new ArgsEqualExpectation(new FakeInvocation(m), new object[0]);
			SetupExpectation(expectation, r, actual);
			return expectation;
		}

		[Test]
		public void ArgsEqualWithDifferentNumberOfParameters()
		{
			IExpectation expectation = new ArgsEqualExpectation(new FakeInvocation(method), new object[] {1, "43", 5.2f});
			object[] args = new object[] {1, "43"};
			Assert.IsFalse(expectation.IsExpected(args));
			Assert.AreEqual("IDemo.VoidThreeArgs(1, \"43\", 5.2);", expectation.ErrorMessage);
		}

		[Test]
		public void ArgsEqualWhenArgsMatch()
		{
			IExpectation expectation = new ArgsEqualExpectation(new FakeInvocation(method), new object[] {1, "43", 5.2f});
			object[] args = new object[] {1, "43", 5.2f};
			Assert.IsTrue(expectation.IsExpected(args));
		}
		
		[Test]
		public void ArgsEqualWhenValueTypeArrayArgsMatch()
		{
			MethodInfo method = typeof (IDemo).GetMethod("VoidValueTypeArrayArgs");
			IExpectation expectation = new ArgsEqualExpectation(new FakeInvocation(method), new object[] { new ushort[] { 123, 456 } });
			object[] args = new object[] { new ushort[] { 123 } };
			Assert.IsFalse(expectation.IsExpected(args));
			Assert.AreEqual("IDemo.VoidValueTypeArrayArgs([123, 456]);", expectation.ErrorMessage);
		}

		[Test]
		public void ArgsEqualFalseWhenMatchingAnotherExpectation()
		{
			IExpectation expectation = new ArgsEqualExpectation(new FakeInvocation(method), new object[] {1, "43", 5.2f});
			IExpectation other = new AnyArgsExpectation(new FakeInvocation(method));
			Assert.AreNotEqual(expectation,other);
		}

		[Test]
		public void ArgsEqualsReturnsTheExpectedArgs()
		{
			object[] args = new object[] {1, "43", 5.2f};
			ArgsEqualExpectation expectation = new ArgsEqualExpectation(new FakeInvocation(method), args);
			Assert.IsTrue(List.Equal(args).Eval(expectation.ExpectedArgs));
		}

		[Test]
		public void ArgsEqualWhenArgsAreNull()
		{
			IExpectation expectation = new ArgsEqualExpectation(new FakeInvocation(method), new object[] {1, null, 5.2f});
			object[] args = new object[] {1, null, 5.2f};
			Assert.IsTrue(expectation.IsExpected(args));
		}

		[Test]
		public void ArgsEqualWhenArgsMismatch()
		{
			IExpectation expectation = new ArgsEqualExpectation(new FakeInvocation(method), new object[] {1, "43", 5.2f});
			object[] args = new object[] {1, "43", 6.4f};
			Assert.IsFalse(expectation.IsExpected(args));
			Assert.AreEqual("IDemo.VoidThreeArgs(1, \"43\", 5.2);", expectation.ErrorMessage);
		}

		[Test]
		public void ArgsEqualWithArrayReferenceEqual()
		{
			object[] arr = new object[3] {"1", 2, 5.2f};
			IExpectation expectation = new ArgsEqualExpectation(new FakeInvocation(method), new object[] {1, arr});
			object[] args = new object[] {1, arr};
			Assert.IsTrue(expectation.IsExpected(args));
		}

		[Test]
		public void ArgsEqualWithArrayContentEqual()
		{
			object[] arr1 = new object[3] {"1", 2, 4.5f},
				arr2 = new object[3] {"1", 2, 4.5f};
			IExpectation expectation = new ArgsEqualExpectation(new FakeInvocation(method), new object[] {1, arr2});
			object[] args = new object[] {1, arr1};
			Assert.IsTrue(expectation.IsExpected(args));
		}

		[Test]
		public void ArgsEqualWithArrayContentDifferent()
		{
			object[] arr1 = new object[3] {"1", 2, 4.5f},
				arr2 = new object[3] {"1", 5, 4.5f};
			IExpectation expectation = new ArgsEqualExpectation(new FakeInvocation(method), new object[] {1, arr1, 3});
			object[] args = new object[] {1, arr2, 3};
			Assert.IsFalse(expectation.IsExpected(args));
			Assert.AreEqual("IDemo.VoidThreeArgs(1, [\"1\", 2, 4.5], 3);", expectation.ErrorMessage);
		}

		[Test]
		public void CreateErrorMessageWhenParametersAreNull()
		{
			ArgsEqualExpectation expectation =new ArgsEqualExpectation(new FakeInvocation(method), new object[]{1,null, 3.3f});
			Assert.AreEqual("IDemo.VoidThreeArgs(1, null, 3.3);",expectation.ErrorMessage);
		}

		[Test]
		public void ArgsEqualWithArrayContentLengthDifferent()
		{
			object[] arr1 = new object[3] {"1", 2, 4.5f},
				arr2 = new object[2] {"1", 5};
			IExpectation expectation = new ArgsEqualExpectation(new FakeInvocation(method), new object[] {1, arr1});
			object[] args = new object[] {1, arr2};
			Assert.IsFalse(expectation.IsExpected(args));
			Assert.AreEqual("IDemo.VoidThreeArgs(1, [\"1\", 2, 4.5], missing parameter);", expectation.ErrorMessage);
		}

		[Test]
		public void ArgsEqualWithStringArray()
		{
			MethodInfo method = typeof (IDemo).GetMethod("VoidThreeStringArgs");
			string[] str1 = new string[] {"", "1", "1234"},
				str2 = new string[] {"1", "1234", "54321"};
			IExpectation expectation = new ArgsEqualExpectation(new FakeInvocation(method), str1);
			Assert.IsFalse(expectation.IsExpected(str2));
			Assert.AreEqual("IDemo.VoidThreeStringArgs(\"\", \"1\", \"1234\");", expectation.ErrorMessage);
		}
	}
}