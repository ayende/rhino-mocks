using System.Collections.Generic;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Generated
{
	/// <summary>
	/// Dictionary class
	/// </summary>
	public class ProxyStateDictionary : Dictionary<object, IMockState>
	{
		/// <summary>
		/// Create a new instance of <c>ProxyStateDictionary</c>
		/// </summary>
		public ProxyStateDictionary() : base(MockedObjectsEquality.Instance)
		{
		}
	}
}