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
using System.ComponentModel;
using System.Text;

namespace Rhino.Mocks.Constraints
{

	/// <summary>
	/// Provides access to the constraints defined in the class <see cref="Is"/> to be used in context
	/// with the <see cref="Arg&lt;T&gt;"/> syntax.
	/// </summary>
	/// <typeparam name="T">The type of the argument</typeparam>
	public class IsArg<T>
	{

		internal IsArg() { }


		/// <summary>
		/// Evaluate a greater-than constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be greater than</param>
		public T GreaterThan(IComparable objToCompare)
		{
            objToCompare = ConvertObjectTypeToMatch(objToCompare);
			ArgManager.AddInArgument(Is.GreaterThan(objToCompare));
			return default(T);
		}

		/// <summary>
		/// Evaluate a less-than constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be less than</param>
		public T LessThan(IComparable objToCompare)
		{
            objToCompare = ConvertObjectTypeToMatch(objToCompare);
			ArgManager.AddInArgument(Is.LessThan(objToCompare));
			return default(T);
		}

		/// <summary>
		/// Evaluate a less-than-or-equal constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be less than or equal to</param>
		public T LessThanOrEqual(IComparable objToCompare)
		{
		    objToCompare = ConvertObjectTypeToMatch(objToCompare);
			ArgManager.AddInArgument(Is.LessThanOrEqual(objToCompare));
			return default(T);
		}

		/// <summary>
		/// Evaluate a greater-than-or-equal constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be greater than or equal to</param>
		public T GreaterThanOrEqual(IComparable objToCompare)
		{
            objToCompare = ConvertObjectTypeToMatch(objToCompare);
			ArgManager.AddInArgument(Is.GreaterThanOrEqual(objToCompare));
			return default(T);
		}

		/// <summary>
		/// Evaluate an equal constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="obj">The object the parameter should equal to</param>
		public T Equal(object obj)
		{
            obj = ConvertObjectTypeToMatch(obj);
			ArgManager.AddInArgument(Is.Equal(obj));
			return default(T);
		}

        /// <summary>
        /// Converts the <see cref="IComparable" />object type to a better match if this is a primitive type.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        private IComparable ConvertObjectTypeToMatch(IComparable obj)
        {
            var obj2 = ConvertObjectTypeToMatch(obj as object);
            if (obj2 is IComparable)
            {
                obj = obj2 as IComparable;
            }

            return obj;
        }

        /// <summary>
        /// Converts the object type to match.
        /// </summary>
        /// <remarks>
        /// Because of implicit conversions and the way ArgConstraints this method is needed to check
        /// object type and potentially change the object type for a better "match" so that obj1.Equals(obj2)
        /// will return the proper "answer"
        /// </remarks>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
	    private object ConvertObjectTypeToMatch(object obj)
	    {
	        if (typeof(T).IsPrimitive && typeof(T) != obj.GetType())
	        {
	            try
	            {
	                obj = Convert.ChangeType(obj, typeof(T));
	            }
	            catch (Exception ex)
	            {
	                if (ex is InvalidCastException || ex is FormatException)
	                {

	                }
	                else
	                {
	                    throw;
	                }
	            }

	        }
	        return obj;
	    }


	    /// <summary>
		/// Evaluate a not equal constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="obj">The object the parameter should not equal to</param>
		public T NotEqual(object obj)
		{
            obj = ConvertObjectTypeToMatch(obj);
			ArgManager.AddInArgument(Is.NotEqual(obj));
			return default(T);
		}


		/// <summary>
		/// Evaluate a same-as constraint.
		/// </summary>
		/// <param name="obj">The object the parameter should the same as.</param>
		public T Same(object obj)
		{
            obj = ConvertObjectTypeToMatch(obj);
			ArgManager.AddInArgument(Is.Same(obj));
			return default(T);
		}

		/// <summary>
		/// Evaluate a not-same-as constraint.
		/// </summary>
		/// <param name="obj">The object the parameter should not be the same as.</param>
		public T NotSame(object obj)
		{
			ArgManager.AddInArgument(Is.NotSame(obj));
			return default(T);
		}

		/// <summary>
		/// A constraint that accepts anything
		/// </summary>
		/// <returns></returns>
		public T Anything
		{
			get
			{
				ArgManager.AddInArgument(Is.Anything());
				return default(T);
			}
		}

		/// <summary>
		/// A constraint that accepts only nulls
		/// </summary>
		/// <returns></returns>
		public T Null
		{
			get
			{
				ArgManager.AddInArgument(Is.Null());
				return default(T);
			}
		}

		/// <summary>
		/// A constraint that accepts only non null values
		/// </summary>
		/// <returns></returns>
		public T NotNull
		{
			get
			{
				ArgManager.AddInArgument(Is.NotNull());
				return default(T);
			}
		}

		/// <summary>
		/// A constraint that accepts only values of the specified type.
		/// The check is performed on the type that has been defined
		/// as the argument type.
		/// </summary>
		public T TypeOf
		{
			get
			{
				ArgManager.AddInArgument(Is.TypeOf<T>());
				return default(T);
			}
		}

		/// <summary>
		/// Throws NotSupportedException. Don't use Equals to define constraints. Use Equal instead.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			throw new InvalidOperationException("Don't use Equals() to define constraints, use Equal() instead");
		}

		/* implement GetHashCode to avoid compiler warning */
		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

}
