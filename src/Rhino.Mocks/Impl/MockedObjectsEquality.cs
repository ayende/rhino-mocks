using System;
using System.Collections;
using System.Threading;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// This class will provide hash code for hashtables without needing
	/// to call the GetHashCode() on the object, which may very well be mocked.
	/// This class has no state so it is a singelton to avoid creating a lot of objects 
	/// that does the exact same thing. See flyweight patterns.
	/// </summary>
    public class MockedObjectsEquality : IComparer,
#if dotNet2
    IEqualityComparer, System.Collections.Generic.IEqualityComparer<object>
#else
        IHashCodeProvider
#endif   
    
	{
		private static MockedObjectsEquality instance = new MockedObjectsEquality();

		private static int baseHashcode = 0;

		/// <summary>
		/// The next hash code value for a mock object.
		/// This is safe for multi threading.
		/// </summary>
		public static int NextHashCode
		{
			get
			{
				return Interlocked.Increment(ref baseHashcode);
			}
		}

		/// <summary>
        /// The sole instance of <see cref="MockedObjectsEquality "/>
		/// </summary>
		public static MockedObjectsEquality Instance
		{
			get { return instance; }
		}

		/// <summary>
		/// Get the hash code for a proxy object without calling GetHashCode()
		/// on the object.
		/// </summary>
		public int GetHashCode(object obj)
		{
			IMockedObject mockedObject = MockRepository.GetMockedObjectOrNull(obj);
			if (mockedObject==null)
				return obj.GetHashCode();
			return mockedObject.ProxyHash;
		}

        /// <summary>
        /// Compares two instances of mocked objects
        /// </summary>
        public int Compare(object x, object y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return 1;
            if (y == null)
                return -1;

			IMockedObject one = MockRepository.GetMockedObjectOrNull(x);
			IMockedObject two = MockRepository.GetMockedObjectOrNull(y);
            if (one == null && two == null)
                throw new ArgumentException("Both arguments to Compare() were not Mocked objects!");
            if (one == null)
                return 1;
            if (two == null)
                return -1;

            return one.ProxyHash - two.ProxyHash;
        }

		private MockedObjectsEquality()
		{
		}

        #region IEqualityComparer Members

        /// <summary>
        /// Compare two mocked objects
        /// </summary>
        public new bool Equals(object x, object y)
        {
            return Compare(x, y)==0;
        }

        #endregion

        #region IEqualityComparer<object> Members
#if dotNet2
        bool System.Collections.Generic.IEqualityComparer<object>.Equals(object x, object y)
        {
            return Compare(x, y) == 0;
        }

        int System.Collections.Generic.IEqualityComparer<object>.GetHashCode(object obj)
        {
            return this.GetHashCode(obj);
        }
#endif
        #endregion
    }
}
