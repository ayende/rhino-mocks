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
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.MethodRecorders;
using Rhino.Mocks.Tests.Expectations;

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
			expectation = new AnyArgsExpectation(new FakeInvocation(endsWith), new Range(1, 1));
			proxy = new ProxyInstance(null);
		}

		[Test]
		public void EqualsTest()
		{
			ProxyInstance proxy1 = new ProxyInstance(null);
			ProxyInstance proxy2 = new ProxyInstance(null);
			MethodInfo method1 = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }),
				method2 = endsWith;
			IExpectation expectation1 = new AnyArgsExpectation(new FakeInvocation(method1), new Range(1, 1)),
				expectation2 = new AnyArgsExpectation(new FakeInvocation(method2), new Range(1, 1));
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
		[ExpectedException(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: proxy")]
		public void ProxyNullThrows()
		{
			new ProxyMethodExpectationTriplet(null, endsWith, expectation);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: method")]
		public void MethodNullThrows()
		{
			new ProxyMethodExpectationTriplet(proxy, null, expectation);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: expectation")]
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
			Assert.AreEqual(triplet.GetHashCode(), triplet.GetHashCode());
		}
	}
}