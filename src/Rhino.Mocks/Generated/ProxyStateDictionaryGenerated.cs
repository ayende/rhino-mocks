using System.Collections;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Generated
{
#if dotNet2
    /// <summary>
	/// Dictionary class
	/// </summary>
    public class ProxyStateDictionary : System.Collections.Generic.Dictionary<object,IMockState>
    {
        /// <summary>
        /// Create a new instance of <c>ProxyStateDictionary</c>
        /// </summary>
        public ProxyStateDictionary() : base(MockedObjectsEquality.Instance) { }
    }
#else
	/// <summary>
	/// Dictionary class
	/// </summary>
	public class ProxyStateDictionary
	{
		Hashtable inner;

		/// <summary>
		/// Create a new instance
		/// </summary>
		public ProxyStateDictionary()
		{
			inner = new Hashtable(MockedObjectsEquality.Instance, MockedObjectsEquality.Instance);	
		}

		/// <summary>
		/// Add proxy and state
		/// </summary>
		public void Add(object mock, IMockState state)
		{
			inner.Add(mock, state);
		}

		/// <summary>
		/// Contains a proxy
		/// </summary>
		public bool ContainsKey(object mock)
		{
			return inner.Contains(mock);
		}

		
		/// <summary>
		/// Keys
		/// </summary>
		public ICollection Keys
		{
			get
			{
				return inner.Keys;
			}
		}

		/// <summary>
		/// Indexer
		/// </summary>
		public IMockState this[object mock]
		{
			get
			{
				return (IMockState)inner[mock];
			}
			set
			{
				inner[mock] = value;
			}
		}

		/// <summary>
		/// Number of items
		/// </summary>
		public int Count
		{
			get { return Keys.Count; }
		}
	}
#endif
}