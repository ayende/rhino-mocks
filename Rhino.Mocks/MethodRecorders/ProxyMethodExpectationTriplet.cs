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
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.MethodRecorders
{
	/// <summary>
	/// Hold an expectation for a method call on an object
	/// </summary>
	public class ProxyMethodExpectationTriplet
	{
		private object proxy;
		private MethodInfo method;
		private IExpectation expectation;
		
		/// <summary>
		/// Creates a new <see cref="ProxyMethodExpectationTriplet"/> instance.
		/// </summary>
		/// <param name="proxy">Proxy.</param>
		/// <param name="method">Method.</param>
		/// <param name="expectation">Expectation.</param>
		public ProxyMethodExpectationTriplet(object proxy, MethodInfo method, IExpectation expectation)
		{
			Validate.IsNotNull(proxy, "proxy");
			Validate.IsNotNull(method, "method");
			Validate.IsNotNull(expectation, "expectation");
			this.proxy = proxy;
			this.method = method;
			this.expectation = expectation;
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
		/// Gets the expectation.
		/// </summary>
		/// <value></value>
		public IExpectation Expectation
		{
			get { return expectation; }
			set { expectation = value; }
		}

		/// <summary>
		/// Determines if the object equal to this instance
		/// </summary>
		/// <param name="obj">Obj.</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			ProxyMethodExpectationTriplet other = obj as ProxyMethodExpectationTriplet;
			if (other == null)
				return false;
			return method == other.method &&
                MockedObjectsEquality.Instance.Compare(proxy, other.proxy) == 0 &&
				expectation == other.expectation;
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return method.GetHashCode() + MockedObjectsEquality.Instance.GetHashCode(proxy) + expectation.GetHashCode();
		}
	}
}
