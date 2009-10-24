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


using System.Reflection;
using Xunit;
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

		[Fact]
		public void ExpectationEqualToSameTypeWithSameArgs()
		{
			IExpectation one = GetExpectation(method, new Range(2,3), 4);
			IExpectation two = GetExpectation(method, new Range(2,3), 4);
			Assert.NotSame(one,two);
			Assert.Equal(one,two);
		}

		[Fact]
		public void ExpectationNotEqualToNull()
		{
			IExpectation one = GetExpectation(method, new Range(2,3), 4);
			Assert.False(one.Equals(null));
	
		}

		[Fact]
		public void AbstractExpectationPropertiesReturnTheValuesSetByDerivedClass()
		{
			Range r = new Range(0, 30);
			IExpectation test = GetExpectation(method, r, 5);
			Assert.Equal(r, test.Expected);
			Assert.Equal(5, test.ActualCallsCount);
		}


		[Fact]
		public void SettingMethodRepeatToAnyMeansThatCanAlwaysAcceptCalls()
		{
			IExpectation test = GetExpectation(method, new Range(0,0), 5);
			Assert.False(test.CanAcceptCalls);
			test.RepeatableOption = RepeatableOption.Any;
			Assert.True(test.CanAcceptCalls);
		}

		[Fact]
		public void SettingReaptableToAnyMeansThatExceptionIsAlwaysSatisfied()
		{
			IExpectation test = GetExpectation(method, new Range(0,1), 5);
			Assert.False(test.ExpectationSatisfied);
			test.RepeatableOption = RepeatableOption.Any;
			Assert.True(test.ExpectationSatisfied);
	
		}

		[Fact]
		public void HasCallLeftWhenThereIsACallLeft()
		{
			Range r = new Range(0, 1);
			IExpectation test = GetExpectation(method, r, 0);
			Assert.True(test.CanAcceptCalls);
		}


		[Fact]
		public void HasCallLeftWhenThereArentCallLeft()
		{
			Range r = new Range(0, 1);
			IExpectation test = GetExpectation(method, r, 1);
			Assert.False(test.CanAcceptCalls);
		}

		[Fact]
		public void AddActualMethodIncreaseActualCalls()
		{
			Range r = new Range(0, 5);
			IExpectation test = GetExpectation(method, r, 0);
			Assert.Equal(0, test.ActualCallsCount);
            test.AddActualCall();

			Assert.Equal(1, test.ActualCallsCount);
            test.AddActualCall();
            Assert.Equal(2, test.ActualCallsCount);
		}

		[Fact]
		public void ExpectationIsClosedForVoidMethod()
		{
			Range r = new Range(0, 5);
			MethodInfo voidMethod = typeof (IDemo).GetMethod("VoidNoArgs");
			IExpectation test = GetExpectation(voidMethod, r, 0);
			Assert.True(test.ActionsSatisfied);
		}

		[Fact]
		public void HashCodeIsTheSameAcrossInvocations()
		{
			Range r = new Range(0, 5);
			MethodInfo voidMethod = typeof (IDemo).GetMethod("VoidNoArgs");
			IExpectation test = GetExpectation(voidMethod, r, 0);
			Assert.Equal(test.GetHashCode(),test.GetHashCode());
		}
	}
}