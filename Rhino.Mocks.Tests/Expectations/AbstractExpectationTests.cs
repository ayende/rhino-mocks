using System.Reflection;
using MbUnit.Framework;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.Expectations
{

	public abstract class AbstractExpectationTests
	{
		private MethodInfo method = typeof (IDemo).GetMethod("VoidNoArgs");

		protected abstract IExpectation GetExpectation(MethodInfo m, Range r, int actual);

		protected static void SetupExpectation(IExpectation expectation, Range r, int actual)
		{
			expectation.Expected = r;
			for (int i = 0; i < actual; i++)
			{
				expectation.AddActualCall();
			}
		}

		[Test]
		public void ExpectationEqualToSameTypeWithSameArgs()
		{
			IExpectation one = GetExpectation(method, new Range(2,3), 4);
			IExpectation two = GetExpectation(method, new Range(2,3), 4);
			Assert.AreNotSame(one,two);
			Assert.AreEqual(one,two);
		}

		[Test]
		public void ExpectationNotEqualToNull()
		{
			IExpectation one = GetExpectation(method, new Range(2,3), 4);
			Assert.IsFalse(one.Equals(null));
	
		}

		[Test]
		public void AbstractExpectationPropertiesReturnTheValuesSetByDerivedClass()
		{
			Range r = new Range(0, 30);
			IExpectation test = GetExpectation(method, r, 5);
			Assert.AreEqual(r, test.Expected);
			Assert.AreEqual(5, test.ActualCalls);
		}


		[Test]
		public void SettingMethodRepeatToAnyMeansThatCanAlwaysAcceptCalls()
		{
			IExpectation test = GetExpectation(method, new Range(0,0), 5);
			Assert.IsFalse(test.CanAcceptCalls);
			test.RepeatableOption = RepeatableOption.Any;
			Assert.IsTrue(test.CanAcceptCalls);
		}

		[Test]
		public void SettingReaptableToAnyMeansThatExceptionIsAlwaysSatisfied()
		{
			IExpectation test = GetExpectation(method, new Range(0,1), 5);
			Assert.IsFalse(test.ExpectationSatisfied);
			test.RepeatableOption = RepeatableOption.Any;
			Assert.IsTrue(test.ExpectationSatisfied);
	
		}

		[Test]
		public void HasCallLeftWhenThereIsACallLeft()
		{
			Range r = new Range(0, 1);
			IExpectation test = GetExpectation(method, r, 0);
			Assert.IsTrue(test.CanAcceptCalls);
		}


		[Test]
		public void HasCallLeftWhenThereArentCallLeft()
		{
			Range r = new Range(0, 1);
			IExpectation test = GetExpectation(method, r, 1);
			Assert.IsFalse(test.CanAcceptCalls);
		}

		[Test]
		public void AddActualMethodIncreaseActualCalls()
		{
			Range r = new Range(0, 5);
			IExpectation test = GetExpectation(method, r, 0);
			Assert.AreEqual(0, test.ActualCalls);
			test.AddActualCall();
			Assert.AreEqual(1, test.ActualCalls);
			test.AddActualCall();
			Assert.AreEqual(2, test.ActualCalls);
		}

		[Test]
		public void ExpectationIsClosedForVoidMethod()
		{
			Range r = new Range(0, 5);
			MethodInfo voidMethod = typeof (IDemo).GetMethod("VoidNoArgs");
			IExpectation test = GetExpectation(voidMethod, r, 0);
			Assert.IsTrue(test.ActionsSatisfied);
		}

		[Test]
		public void HashCodeIsTheSameAcrossInvocations()
		{
			Range r = new Range(0, 5);
			MethodInfo voidMethod = typeof (IDemo).GetMethod("VoidNoArgs");
			IExpectation test = GetExpectation(voidMethod, r, 0);
			Assert.AreEqual(test.GetHashCode(),test.GetHashCode());
		}
	}
}