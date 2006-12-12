using System;
using Rhino.Mocks.Interfaces;

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
			return RecursiveArrayEqual(expectedArgs, actualArgs);
		}

		#region Implementation

        private static bool RecursiveArrayEqual(Array expectedArgs, Array actualArgs)
        {
            if (expectedArgs.Length != actualArgs.Length)
                return false;

            for (int i = 0; i < expectedArgs.Length; i++)
            {
                if (expectedArgs.GetValue(i) == null)
                {
                    if (actualArgs.GetValue(i) == null)
                        continue;
                    else
                        return false;
                }
                object expected = expectedArgs.GetValue(i);
                object actual = actualArgs.GetValue(i);
                if (SafeEquals(expected, actual))
                    continue;
                if (expectedArgs.GetValue(i) is Array)
                {
                    if (RecursiveArrayEqual((Array)expectedArgs.GetValue(i),
                            (Array)actualArgs.GetValue(i)) == false)
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