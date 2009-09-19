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


using System.Collections;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Constraints
{
	/*
	 * class: List
	 * 
	 * Constraints for dealing with lists.
	 * 
	 */ 
	/// <summary>
	/// Central location for constraints about lists and collections
	/// </summary>
	public static class List
	{

		/*
		 * Method: IsIn
		 * 
		 * Determines whether the specified obj is in the parameter.
		 * The parameter must be IEnumerable.
		 */ 
		/// <summary>
		/// Determines whether the specified obj is in the parameter.
		/// The parameter must be IEnumerable.
		/// </summary>
		/// <param name="obj">Obj.</param>
		/// <returns></returns>
		public static AbstractConstraint IsIn(object obj)
		{
			return new IsIn(obj);
		}

		/*
		 * Method: OneOf
		 * 
		 * Determines whatever the parameter is in the collection.
		 */ 
		/// <summary>
		/// Determines whatever the parameter is in the collection.
		/// </summary>
		public static AbstractConstraint OneOf(IEnumerable collection)
		{
			return new OneOf(collection);
		}

		/*
		 * Method Equal
		 * Determines that the parameter collection is identical to the specified collection
		 * This is done by iterating the collections and comparing each element.
		 */ 
		/// <summary>
		/// Determines that the parameter collection is identical to the specified collection
		/// </summary>
		public static AbstractConstraint Equal(IEnumerable collection)
		{
			return new CollectionEqual(collection);
		}

        /// <summary>
        /// Determines that the parameter collection has the specified number of elements.
        /// </summary>
        /// <param name="constraint">The constraint that should be applied to the collection count.</param>
        public static AbstractConstraint Count(AbstractConstraint constraint)
        {
            return new CollectionCount(constraint);
        }

        /// <summary>
        /// Determines that an element of the parameter collections conforms to another AbstractConstraint.
        /// </summary>
        /// <param name="index">The zero-based index of the list element.</param>
        /// <param name="constraint">The constraint which should be applied to the list element.</param>
        public static AbstractConstraint Element(int index, AbstractConstraint constraint)
        {
            return new ListElement(index, constraint);
        }

        /// <summary>
        /// Determines that an element of the parameter collections conforms to another AbstractConstraint.
        /// </summary>
        /// <param name="key">The key of the element.</param>
        /// <param name="constraint">The constraint which should be applied to the element.</param>
        public static AbstractConstraint Element<T>(T key, AbstractConstraint constraint)
        {
            return new KeyedListElement<T>(key, constraint);
        }

        /*
         * Method ContainsAll
         * Determines that all elements of the specified collection are in the the parameter collection 
         */
        ///<summary>
        /// Determines that all elements of the specified collection are in the the parameter collection 
        ///</summary>
        ///<param name="collection">The collection to compare against</param>
        ///<returns>The constraint which should be applied to the list parameter.</returns>
        public static AbstractConstraint ContainsAll(IEnumerable collection)
        {
            return new ContainsAll(collection);
        }
	}
}
