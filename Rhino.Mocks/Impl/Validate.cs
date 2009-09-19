#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using Rhino.Mocks.Interfaces;
using System.Collections;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Validate arguments for methods
	/// </summary>
	public static class Validate
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

		/// <summary>
		/// Validate that the two arguments are equals, including validation for
		/// when the arguments are collections, in which case it will validate their values.
		/// </summary>
		public static bool AreEqual(object expectedArg, object actualArg)
		{
			return RecursiveCollectionEqual(new object[] { expectedArg }, new object[] { actualArg });
		}

		#region Implementation

        private static bool RecursiveCollectionEqual(ICollection expectedArgs, ICollection actualArgs)
        {
            if(expectedArgs == null && actualArgs == null)
                return true;
            if(expectedArgs==null || actualArgs==null)
                return false;

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
