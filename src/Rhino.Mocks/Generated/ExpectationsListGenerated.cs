using System;
using System.Collections;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Generated
{

#if dotNet2
     /// <summary>
    /// ExpectationsList
    /// </summary>
    public class ExpectationsList : System.Collections.Generic.List<IExpectation>
    {
    }

#else
    /// <summary>
    /// ExpectationsList
    /// </summary>
    public class ExpectationsList : IEnumerable
    {
        ArrayList inner = new ArrayList();
        /// <summary>
        /// Add a single expectation
        /// </summary>
        public int Add(IExpectation expectation)
        {
            return inner.Add(expectation);
        }

        /// <summary>
        /// Add a range of expectations
        /// </summary>
        public void AddRange(ExpectationsList expectations)
        {
            inner.AddRange(expectations.inner);
        }

        /// <summary>
        /// Indexer
        /// </summary>
        public IExpectation this[int index]
        {
            get
            {
                return (IExpectation)inner[index];
            }
            set
            {
				inner[index] = value;
            }
        }

        /// <summary>
        /// Whatever the collection contains the expectation
        /// </summary>
        public bool Contains(IExpectation expectation)
        {
            return inner.Contains(expectation);
        }
        
        ///<summary>
        ///The index of the expectation
        ///</summary>
        public int IndexOf(IExpectation expectation)
        {
			return inner.IndexOf(expectation);
        }

        /// <summary>
        /// count of items in list
        /// </summary>
        public int Count
        {
            get { return inner.Count; }
        }

        /// <summary>
        /// enumerator
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return inner.GetEnumerator();
        }
    }
#endif
	
}