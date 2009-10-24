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
using Xunit;
using Rhino.Mocks.Impl;
using Rhino.Mocks.MethodRecorders;

namespace Rhino.Mocks.Tests.MethodRecorders
{
	
	public class ProxyMethodPairTests
	{
        private MethodInfo endsWith = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
        private MethodInfo startsWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });

		[Fact]
		public void CtorValueReturnedInProperties()
		{
			object mockProxy = new object(); // mock for the mocked, he he
			ProxyMethodPair pair = new ProxyMethodPair(mockProxy, endsWith);
			Assert.Same(mockProxy, pair.Proxy);
			Assert.Same(endsWith, pair.Method);
		}

		[Fact]
		public void EqualsToAnotherProxy()
		{
			ProxyInstance mockProxy = new ProxyInstance(null);
			ProxyMethodPair pair1 = new ProxyMethodPair(mockProxy, endsWith);
			ProxyMethodPair pair2 = new ProxyMethodPair(mockProxy, endsWith);
			Assert.Equal(pair1, pair2);
			Assert.Equal(pair2, pair1); //make sure that it works both ways
		}

		[Fact]
		public void NotEqualToNull()
		{
			ProxyInstance mockProxy = new ProxyInstance(null);
			ProxyMethodPair pair1 = new ProxyMethodPair(mockProxy, endsWith);
			Assert.False(pair1.Equals(null));
		}

		[Fact]
		public void NotEqualIfNotSameObject()
		{
			ProxyInstance mockProxy1 = new ProxyInstance(null);
			ProxyInstance mockProxy2 = new ProxyInstance(null);
			ProxyMethodPair pair1 = new ProxyMethodPair(mockProxy1, endsWith);
			ProxyMethodPair pair2 = new ProxyMethodPair(mockProxy2, endsWith);
			Assert.NotEqual(pair1, pair2);
			Assert.NotEqual(pair2, pair1); //make sure that it works both ways
		}

		[Fact]
		public void NotEqualIfNotSameMethod()
		{
			ProxyInstance mockProxy = new ProxyInstance(null);
			ProxyMethodPair pair1 = new ProxyMethodPair(mockProxy, endsWith);
			ProxyMethodPair pair2 = new ProxyMethodPair(mockProxy, startsWith);
			Assert.NotEqual(pair1, pair2);
			Assert.NotEqual(pair2, pair1); //make sure that it works both ways
		}

		[Fact]
		public void NotEqualIfNotSameMethodAndNotSameProxy()
		{
			ProxyInstance mockProxy1 = new ProxyInstance(null);
			ProxyInstance mockProxy2 = new ProxyInstance(null);
			ProxyMethodPair pair1 = new ProxyMethodPair(mockProxy1, this.endsWith);
			ProxyMethodPair pair2 = new ProxyMethodPair(mockProxy2, this.startsWith);
			Assert.NotEqual(pair1, pair2);
			Assert.NotEqual(pair2, pair1); //make sure that it works both ways
		}

		[Fact]
		public void ProxyNullThrows()
		{
			Assert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: proxy", () => new ProxyMethodPair(null, endsWith));
		}

		[Fact]
		public void MethodNullThrows()
		{
			ProxyInstance mockProxy = new ProxyInstance(null);

			Assert.Throws<ArgumentNullException>("Value cannot be null.\r\nParameter name: method",
			                                     () => new ProxyMethodPair(mockProxy, null));
		}
	}
}