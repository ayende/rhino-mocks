using System;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.MethodRecorders;

namespace Rhino.Mocks.Tests.MethodRecorders
{
	[TestFixture]
	public class ProxyMethodExpectationTripletTests
	{
		private MethodInfo endsWith;
		private AnyArgsExpectation expectation;
		private ProxyInstance proxy;

		[SetUp]
		public void SetUp()
		{
            endsWith = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
			expectation = new AnyArgsExpectation(endsWith);
			proxy = new ProxyInstance(null);
		}

		[Test]
		public void EqualsTest()
		{
			ProxyInstance proxy1 = new ProxyInstance(null);
			ProxyInstance proxy2 = new ProxyInstance(null);
            MethodInfo method1 = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }),
				method2 = endsWith;
			IExpectation expectation1 = new AnyArgsExpectation(method1),
				expectation2 = new AnyArgsExpectation(method2);
			ProxyMethodExpectationTriplet same1 = new ProxyMethodExpectationTriplet(proxy1, method1, expectation1),
				same2 = new ProxyMethodExpectationTriplet(proxy1, method1, expectation1);
			Assert.AreEqual(same1, same2);
			Assert.AreEqual(same2, same1);

			ProxyMethodExpectationTriplet proxyDiff1 = new ProxyMethodExpectationTriplet(proxy1, method1, expectation1),
				proxyDiff2 = new ProxyMethodExpectationTriplet(proxy2, method1, expectation1);
			Assert.AreNotEqual(proxyDiff2, proxyDiff1);
			Assert.AreNotEqual(proxyDiff1, proxyDiff2);

			ProxyMethodExpectationTriplet methodDiff1 = new ProxyMethodExpectationTriplet(proxy1, method1, expectation1),
				methodDiff2 = new ProxyMethodExpectationTriplet(proxy1, method2, expectation1);

			Assert.AreNotEqual(methodDiff1, methodDiff2);
			Assert.AreNotEqual(methodDiff2, methodDiff1);


			ProxyMethodExpectationTriplet expectationDiff1 = new ProxyMethodExpectationTriplet(proxy1, method1, expectation1),
				expectationDiff2 = new ProxyMethodExpectationTriplet(proxy1, method1, expectation2);

			Assert.AreNotEqual(expectationDiff1, expectationDiff2);
			Assert.AreNotEqual(expectationDiff2, expectationDiff1);


			ProxyMethodExpectationTriplet allDiff1 = new ProxyMethodExpectationTriplet(proxy1, method1, expectation1),
				allDiff2 = new ProxyMethodExpectationTriplet(proxy2, method2, expectation2);

			Assert.AreNotEqual(allDiff1, allDiff2);
			Assert.AreNotEqual(allDiff2, allDiff1);


		}

		[Test]
		public void ReturnSamevaluesAsInCtor()
		{
			ProxyMethodExpectationTriplet triplet = new ProxyMethodExpectationTriplet(proxy, this.endsWith, this.expectation);
			Assert.AreEqual(proxy, triplet.Proxy);
			Assert.AreEqual(endsWith, triplet.Method);
			Assert.AreEqual(expectation, triplet.Expectation);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: proxy")]
		public void ProxyNullThrows()
		{
			new ProxyMethodExpectationTriplet(null, endsWith, expectation);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: method")]
		public void MethodNullThrows()
		{
			new ProxyMethodExpectationTriplet(proxy, null, expectation);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: expectation")]
		public void ExpectationNullThrows()
		{
			new ProxyMethodExpectationTriplet(proxy, endsWith, null);
		}

		[Test]
		public void FalseOnEqualToNull()
		{
			ProxyMethodExpectationTriplet triplet = new ProxyMethodExpectationTriplet(proxy, this.endsWith, this.expectation);
			Assert.IsFalse(triplet.Equals(null));
		}

		[Test]
		public void GetHashCodeReturnSameValue()
		{
			ProxyMethodExpectationTriplet triplet = new ProxyMethodExpectationTriplet(proxy, this.endsWith, this.expectation);
			Assert.AreEqual(triplet.GetHashCode(),triplet.GetHashCode());
		}
	}
}