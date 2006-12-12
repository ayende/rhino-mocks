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
		/// Determains whatever obj equals to this instance.
		/// ProxyMethodPairs are equals when they point to the same /instance/ of
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

        /// <summary>
        /// Gets a string representation of this <see cref="ProxyMethodPair"/>
        /// to assist debugging.
        /// </summary>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "ProxyMethodPair({0},{1})", Proxy, Method);
        }
	}
}
