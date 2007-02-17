using System;
using System.Reflection;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using MbUnit.Framework;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.Expectations
{
	[TestFixture]
	public class AnyArgsExpectationTests : AbstractExpectationTests
	{
		private ArgsEqualExpectation equal;
		private AnyArgsExpectation any;
		private MethodInfo method;

		protected override IExpectation GetExpectation(MethodInfo m, Range r, int actual)
		{
			AnyArgsExpectation expectation = new AnyArgsExpectation(new FakeInvocation(m));
			SetupExpectation(expectation, r, actual);
			return expectation;
		}


		[SetUp]
		public void SetUp()
		{
            method = typeof(int).GetMethod("CompareTo", new Type[] { typeof(object) });
			equal = new ArgsEqualExpectation(new FakeInvocation(this.method), new object[] {1});
			any = new AnyArgsExpectation(this.equal);
		}

		[Test]
		public void AnyArgsExpectationReturnTrueForDifferentArgs()
		{
			Assert.IsFalse(equal.IsExpected(new object[0]));
			Assert.IsTrue(any.IsExpected(new object[0]));
		}

		[Test]
		public void ErrorMessageContainsAnyForParameters()
		{
			string IsExpected = "Int32.CompareTo(any);";
			Assert.AreEqual(IsExpected, any.ErrorMessage);
		}

		[Test]
		public void AnyArgsIsNotEqualsToNonAnyArgsExpectation()
		{
			IExpectation other = new ArgsEqualExpectation(new FakeInvocation(method), new object[0]);
			Assert.AreNotEqual(any, other );
		}

		[Test]
		public void AnyArgsEqualToAnyOtherAnyArgs()
		{
			Assert.AreEqual(any, new AnyArgsExpectation(new FakeInvocation(method)));
		}
	}

	public class FakeInvocation : AbstractInvocation
	{
		public FakeInvocation(MethodInfo targetMethod) 
			: base(null, null, null, null, targetMethod, new object[0])
		{
		}

		protected override void InvokeMethodOnTarget()
		{
			throw new NotImplementedException();
		}
	}
}