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
using System.Globalization;
using System.Reflection;

using Rhino.Mocks.Impl;

namespace Rhino.Mocks.MethodRecorders
{
	/// <summary>
	/// Holds a pair of mocked object and a method
	/// and allows to compare them against each other.
	/// This allows us to have a distinction between mockOne.MyMethod() and
	/// mockTwo.MyMethod()...
	/// </summary>
	public class ProxyMethodPair
	{
		private object proxy;
		private MethodInfo method;

		/// <summary>
		/// Creates a new <see cref="ProxyMethodPair"/> instance.
		/// </summary>
		/// <param name="proxy">Proxy.</param>
		/// <param name="method">Method.</param>
		public ProxyMethodPair(object proxy, MethodInfo method)
		{
			Validate.IsNotNull(proxy, "proxy");
			Validate.IsNotNull(method, "method");
			this.proxy = proxy;
			this.method = method;
		}

		/// <summary>
		/// Gets the proxy.
		/// </summary>
		/// <value></value>
		public object Proxy
		{
			get { return proxy; }
		}

		/// <summary>
		/// Gets the method.
		/// </summary>
		/// <value></value>
		public MethodInfo Method
		{
			get { return method; }
		}

		/// <summary>
		/// Determines whatever obj equals to this instance.
		/// ProxyMethodPairs are equal when they point to the same /instance/ of
		/// an object, and to the same method.
		/// </summary>
		/// <param name="obj">Obj.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			ProxyMethodPair other = obj as ProxyMethodPair;
			if (other == null)
				return false;
            return MockedObjectsEquality.Instance.Compare(other.proxy, proxy) == 0 && 
				other.method == method;

		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return MockedObjectsEquality.Instance.GetHashCode(proxy) + method.GetHashCode();
		}
	}
}
