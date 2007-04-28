using System;
using Rhino.Mocks.Interfaces;
using System.Collections;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Validate arguments for methods
	/// </summary>
	public
#if dotNet2
        static
#else
        sealed 
#endif 
        class Validate
	{
		/// <summary>
		/// Validate that the passed argument is not null.
		/// </summary>
		/// <param name="obj">The object to validate</param>
		/// <param name="name">The name of the argument</param>
		/// <exception cref="ArgumentNullException">
		/// If the obj is null, an ArgumentNullException with the passed name
		/// is thrown.
		/// </exception>
		public static void IsNotNull(object obj, string name)
		{
			if (obj == null)
				throw new ArgumentNullException(name);
		}

		/// <summary>
		/// Validate that the arguments are equal.
		/// </summary>
		/// <param name="expectedArgs">Expected args.</param>
		/// <param name="actualArgs">Actual Args.</param>
		public static bool ArgsEqual(object[] expectedArgs, object[] actualArgs)
		{
			return RecursiveCollectionEqual(expectedArgs, actualArgs);
		}

		#region Implementation

        private static bool RecursiveCollectionEqual(ICollection expectedArgs, ICollection actualArgs)
        {
            if (expectedArgs.Count != actualArgs.Count)
                return false;

            IEnumerator expectedArgsEnumerator = expectedArgs.GetEnumerator();
            IEnumerator actualArgsEnumerator = actualArgs.GetEnumerator();
            while (expectedArgsEnumerator.MoveNext()
                && actualArgsEnumerator.MoveNext())
            {
                object expected = expectedArgsEnumerator.Current;
                object actual = actualArgsEnumerator.Current;

                if (expected == null)
                {
                    if (actual == null)
                        continue;
                    else
                        return false;
                }

                if (SafeEquals(expected, actual))
                    continue;

                if (expected is ICollection)
                {
                    if (!RecursiveCollectionEqual(expected as ICollection, actual as ICollection))
                        return false;

                    continue;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// This method is safe for use even if any of the objects is a mocked object
        /// that override equals.
        /// </summary>
        private static bool SafeEquals(object expected, object actual)
        {
            IMockedObject expectedMock = expected as IMockedObject;
            IMockedObject actualMock = actual as IMockedObject;
            //none are mocked object
            if (expectedMock == null && actualMock == null)
            {
                return expected.Equals(actual);
            }
            //if any of them is a mocked object, use mocks equality
            //this may not be what the user is expecting, but it is needed, because
            //otherwise we get into endless loop.
            return MockedObjectsEquality.Instance.Equals(expected,actual);
        }
		#endregion
	}
}
