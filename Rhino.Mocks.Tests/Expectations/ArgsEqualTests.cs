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