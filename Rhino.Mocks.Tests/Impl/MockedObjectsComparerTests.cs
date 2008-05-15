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
using MbUnit.Framework;
using Rhino.Mocks.Impl;

namespace Rhino.Mocks.Tests.Impl
{
	[TestFixture]	
	public class MockedObjectsComparerTests
	{
		MockRepository mocks;
		IDemo one, two;

		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
			one = (IDemo)mocks.StrictMock(typeof(IDemo));
			two = (IDemo)mocks.StrictMock(typeof(IDemo));
		}

		[Test]
		public void FalseForDifferenceMocks()
		{
            bool condition = MockedObjectsEquality.Instance.Compare(one, two) == 0;
			Assert.IsFalse(condition);
		}

		[Test]
		public void TrueForSameObject()
		{
            bool condition = MockedObjectsEquality.Instance.Compare(one, one) == 0;
			Assert.IsTrue(condition);
		}

		[Test]
		public void FalseForOneMockAndOneNull()
		{
            bool condition = MockedObjectsEquality.Instance.Compare(one, null) == 0;
			Assert.IsFalse(condition);
            condition = MockedObjectsEquality.Instance.Compare(null, one) == 0;
			Assert.IsFalse(condition);
		}

		[Test]
		public void TrueForBothNulls()
		{
            bool condition = MockedObjectsEquality.Instance.Compare(null, null) == 0;
			Assert.IsTrue(condition);
		}

		[Test]
		public void FalseForOneMockAndOneNot()
		{
            bool condition = MockedObjectsEquality.Instance.Compare(one, new object()) == 0;
			Assert.IsFalse(condition);
            condition = MockedObjectsEquality.Instance.Compare(new object(), one) == 0;
			Assert.IsFalse(condition);
		}


	}
}
