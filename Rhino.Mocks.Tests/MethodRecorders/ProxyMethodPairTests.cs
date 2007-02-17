using System;
using System.Reflection;
using MbUnit.Framework;
using Rhino.Mocks.Impl;
using Rhino.Mocks.MethodRecorders;

namespace Rhino.Mocks.Tests.MethodRecorders
{
	[TestFixture]
	public class ProxyMethodPairTests
	{
        private MethodInfo endsWith = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
        private MethodInfo startsWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });

		[Test]
		public void CtorValueReturnedInProperties()
		{
			object mockProxy = new object(); // mock for the mocked, he he
			ProxyMethodPair pair = new ProxyMethodPair(mockProxy, endsWith);
			Assert.AreSame(mockProxy, pair.Proxy);
			Assert.AreSame(endsWith, pair.Method);
		}

		[Test]
		public void EqualsToAnotherProxy()
		{
			ProxyInstance mockProxy = new ProxyInstance(null);
			ProxyMethodPair pair1 = new ProxyMethodPair(mockProxy, endsWith);
			ProxyMethodPair pair2 = new ProxyMethodPair(mockProxy, endsWith);
			Assert.AreEqual(pair1, pair2);
			Assert.AreEqual(pair2, pair1); //make sure that it works both ways
		}

		[Test]
		public void NotEqualToNull()
		{
			ProxyInstance mockProxy = new ProxyInstance(null);
			ProxyMethodPair pair1 = new ProxyMethodPair(mockProxy, endsWith);
			Assert.IsFalse(pair1.Equals(null));
		}

		[Test]
		public void NotEqualIfNotSameObject()
		{
			ProxyInstance mockProxy1 = new ProxyInstance(null);
			ProxyInstance mockProxy2 = new ProxyInstance(null);
			ProxyMethodPair pair1 = new ProxyMethodPair(mockProxy1, endsWith);
			ProxyMethodPair pair2 = new ProxyMethodPair(mockProxy2, endsWith);
			Assert.AreNotEqual(pair1, pair2);
			Assert.AreNotEqual(pair2, pair1); //make sure that it works both ways
		}

		[Test]
		public void NotEqualIfNotSameMethod()
		{
			ProxyInstance mockProxy = new ProxyInstance(null);
			ProxyMethodPair pair1 = new ProxyMethodPair(mockProxy, endsWith);
			ProxyMethodPair pair2 = new ProxyMethodPair(mockProxy, startsWith);
			Assert.AreNotEqual(pair1, pair2);
			Assert.AreNotEqual(pair2, pair1); //make sure that it works both ways
		}

		[Test]
		public void NotEqualIfNotSameMethodAndNotSameProxy()
		{
			ProxyInstance mockProxy1 = new ProxyInstance(null);
			ProxyInstance mockProxy2 = new ProxyInstance(null);
			ProxyMethodPair pair1 = new ProxyMethodPair(mockProxy1, this.endsWith);
			ProxyMethodPair pair2 = new ProxyMethodPair(mockProxy2, this.startsWith);
			Assert.AreNotEqual(pair1, pair2);
			Assert.AreNotEqual(pair2, pair1); //make sure that it works both ways
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: proxy")]
		public void ProxyNullThrows()
		{
			new ProxyMethodPair(null, endsWith);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException), "Value cannot be null.\r\nParameter name: method")]
		public void MethodNullThrows()
		{
			ProxyInstance mockProxy = new ProxyInstance(null);
			
			new ProxyMethodPair(mockProxy, null);
		}
	}
}