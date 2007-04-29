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