using System.Collections;
using Rhino.Mocks.MethodRecorders;
using Rhino.Mocks.Impl;

namespace Rhino.Mocks.Generated
{
#if dotNet2
/// <summary>
	/// Dictionary
	/// </summary>
    public class ProxyMethodExpectationsDictionary : System.Collections.Generic.Dictionary<ProxyMethodPair, ExpectationsList>
    {
    }
#else
	/// <summary>
	/// Dictionary
	/// </summary>
	public class ProxyMethodExpectationsDictionary : IEnumerable
	{
		Hashtable inner;

		/// <summary>
		/// Create a new instance
		/// </summary>
		public ProxyMethodExpectationsDictionary()
		{
			inner = new Hashtable();
		}

		/// <summary>
		/// Add proxy and state
		/// </summary>
		public void Add(ProxyMethodPair pair, ExpectationsList list)
		{
			inner.Add(pair, list);
		}

		/// <summary>
		/// Contains a proxy
		/// </summary>
		public bool ContainsKey(ProxyMethodPair pair)
		{
			return inner.Contains(pair);
		}

		/// <summary>
		/// Indexer
		/// </summary>
		public ExpectationsList this[ProxyMethodPair pair]
		{
			get
			{
				return (ExpectationsList)inner[pair];
			}
		}

        ///<summary>
        ///Enumerator
        ///</summary>
        public IEnumerator GetEnumerator()
        {
            return inner.GetEnumerator();
        }
        
        ///<summary>
        ///get the keys for the collection
        ///</summary>
        public ICollection Keys
        {
            get { return inner.Keys;}
        }

        ///<summary>
        ///remove an item from the collection  
        ///</summary>
        public void Remove(ProxyMethodPair pair)
        {
            inner.Remove(pair);
        }
	}
#endif
}