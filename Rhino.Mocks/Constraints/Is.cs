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
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Constraints
{
	/*
	 * Class: Is
	 * 
	 * Common constraints.
	 */ 
	/// <summary>
	/// Central location for constraints
	/// </summary>
	public static class Is
	{
		/*
		 * method: GreaterThan
		 * 
		 * Determines whatever the parameter is greater than objToCompare.
		 * The parameter must implement IComparable 
		 */ 
		/// <summary>
		/// Evaluate a greater-than constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be greater than</param>
		public static AbstractConstraint GreaterThan(IComparable objToCompare)
		{
			return new ComparingConstraint(objToCompare, true, false);
		}

		/*
		 * method: LessThan
		 * 
		 * Determines whatever the parameter is less than objToCompare.
		 * The parameter must implement IComparable 
		 */ 
		/// <summary>
		/// Evaluate a less-than constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be less than</param>
		public static AbstractConstraint LessThan(IComparable objToCompare)
		{
			return new ComparingConstraint(objToCompare, false, false);
		}

	   /*
		* method: LessThanOrEqual
		* 
		* Determines whatever the parameter is less than or equal to objToCompare.
		* The parameter must implement IComparable 
		*/ 
		/// <summary>
		/// Evaluate a less-than-or-equal constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be less than or equal to</param>
		public static AbstractConstraint LessThanOrEqual(IComparable objToCompare)
		{
			return new ComparingConstraint(objToCompare, false, true);
		}

	   /*
		* method: GreaterThanOrEqual
		* 
		* Determines whatever the parameter is greater than or equal to objToCompare.
		* The parameter must implement IComparable 
		*/ 
		/// <summary>
		/// Evaluate a greater-than-or-equal constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be greater than or equal to</param>
		public static AbstractConstraint GreaterThanOrEqual(IComparable objToCompare)
		{
			return new ComparingConstraint(objToCompare, true, true);
		}

	   /*
		* method: Equal
		* 
		* Determines whatever the parameter equal to obj.
		*/ 
		/// <summary>
		/// Evaluate an equal constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="obj">The object the parameter should equal to</param>
		public static AbstractConstraint Equal(object obj)
		{
			return new Equal(obj);
		}


		/*
		 * method: NotEqual
		 * 
		 * Determines whatever the parameter does not equal to obj.
		 */ 
		/// <summary>
		/// Evaluate a not equal constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="obj">The object the parameter should not equal to</param>
		public static AbstractConstraint NotEqual(object obj)
		{
			return !new Equal(obj);
		}

        /// <summary>
        /// Evaluate a same-as constraint.
        /// </summary>
        /// <param name="obj">The object the parameter should the same as.</param>
        public static AbstractConstraint Same(object obj)
        {
            return new Same(obj);
        }

        /// <summary>
        /// Evaluate a not-same-as constraint.
        /// </summary>
        /// <param name="obj">The object the parameter should not be the same as.</param>
        public static AbstractConstraint NotSame(object obj)
        {
            return !new Same(obj);
        }

		/*
		 * method: Anything
		 * 
		 * This constraint always succeeds
		 */ 
		/// <summary>
		/// A constraint that accepts anything
		/// </summary>
		/// <returns></returns>
		public static AbstractConstraint Anything()
		{
			return new Anything();
		}

		/*
		 * Method: Null
		 * 
		 * Whatever the parameter, as long as it is null
		 * 
		 */
		/// <summary>
		/// A constraint that accepts only nulls
		/// </summary>
		/// <returns></returns>
		public static AbstractConstraint Null()
		{
			return new Equal(null);
		}

		/*
		 * Method: NotNull
		 * 
		 * Whatever parameter, as long as it is not null
		 * 
		 */
		/// <summary>
		/// A constraint that accepts only non null values
		/// </summary>
		/// <returns></returns>
		public static AbstractConstraint NotNull()
		{
			return !new Equal(null);
		}

		/*
		 * Method: TypeOf
		 * 
		 * Whatever parameter, as long as it is of the specified type.
		 * 
		 */
		/// <summary>
		/// A constraint that accepts only values of the specified type
		/// </summary>
		public static AbstractConstraint TypeOf(Type type)
		{
			return new TypeOf(type);
		}
		
        /*
		 * Method: TypeOf
		 * 
		 * Whatever parameter, as long as it is of the specified type.
		 * 
		 */
        /// <summary>
        /// A constraint that accepts only values of the specified type
        /// </summary>
		public static AbstractConstraint TypeOf<T>()
		{
			return new TypeOf(typeof(T));
		}

		/// <summary>
		/// Evaluate a parameter using a predicate
		/// </summary>
		/// <param name="predicate">The predicate to use</param>
		public static AbstractConstraint Matching<T>(Predicate<T> predicate)
		{
			return new PredicateConstraint<T>(predicate);
		}
	}
}
