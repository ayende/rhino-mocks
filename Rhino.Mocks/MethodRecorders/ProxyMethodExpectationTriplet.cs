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
		/// Determains if the object equal to this instance
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
