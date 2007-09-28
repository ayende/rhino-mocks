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


using MbUnit.Framework;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Fabian
	{
		public delegate R Func<A1, R>(A1 a1);

		public delegate void Proc<A1, A2>(A1 a1, A2 a2);

		public delegate int StringInt(string s);

		public interface ICache<TKey, TValue>
		{
			TValue GetValue(TKey key);
			void Add(TKey key, TValue value);
		}

		[Test]
		public void TestExpectCall()
		{
			MockRepository mocks = new MockRepository();
			ICache<string, int> mockCache = mocks.CreateMock<ICache<string, int>>();
			Expect.Call(mockCache.GetValue("a")).Do((Func<string, int>)delegate
			{
				return 1;
			});
			mocks.ReplayAll();

			int i = mockCache.GetValue("a");
			Assert.AreEqual(1,i );

			mocks.VerifyAll();
		}

		[Test]
		public void TestLastCall()
		{
			MockRepository mocks = new MockRepository();
			ICache<string, int> mockCache = mocks.CreateMock<ICache<string, int>>();
			mockCache.Add("a", 1);
			LastCall.Do((Proc<string, int>)delegate
			{
			});
			mocks.ReplayAll();

			mockCache.Add("a", 1);

			mocks.VerifyAll();
		}

		[Test]
		public void TestExpectCallWithNonGenericDelegate()
		{
			MockRepository mocks = new MockRepository();
			ICache<string, int> mockCache = mocks.CreateMock<ICache<string, int>>();
			IMethodOptions<int> opts = Expect.Call(mockCache.GetValue("a"));
			opts.Do(new StringInt(GetValue));
			mocks.ReplayAll();

			int i = mockCache.GetValue("a");

			Assert.AreEqual(2, i);

			mocks.VerifyAll();
		}

		private int GetValue(string s)
		{
			return 2;
		}
	}
}