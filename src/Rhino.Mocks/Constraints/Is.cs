using System;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks
{
	/*
	 * Class: Is
	 * 
	 * Common constraints.
	 */ 
	/// <summary>
	/// Central location for constraints
	/// </summary>
	public
#if dotNet2
    static
#else
    sealed
#endif
    class Is
	{
		/*
		 * method: GreaterThan
		 * 
		 * Determains whatever the parameter is greater than objToCompare.
		 * The parameter must implement IComparable 
		 */ 
		/// <summary>
		/// Evaluate a greater than constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be greater than</param>
		public static AbstractConstraint GreaterThan(IComparable objToCompare)
		{
			return new ComparingConstraint(objToCompare, true, false);
		}

		/*
		 * method: LessThan
		 * 
		 * Determains whatever the parameter is less than objToCompare.
		 * The parameter must implement IComparable 
		 */ 
		/// <summary>
		/// Evaluate a less than constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be less than</param>
		public static AbstractConstraint LessThan(IComparable objToCompare)
		{
			return new ComparingConstraint(objToCompare, false, false);
		}

	   /*
		* method: LessThanOrEqual
		* 
		* Determains whatever the parameter is less than or equal to objToCompare.
		* The parameter must implement IComparable 
		*/ 
		/// <summary>
		/// Evaluate a less than or equal constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be less than or equal to</param>
		public static AbstractConstraint LessThanOrEqual(IComparable objToCompare)
		{
			return new ComparingConstraint(objToCompare, false, true);
		}

	   /*
		* method: GreaterThanOrEqual
		* 
		* Determains whatever the parameter is greater than or equal to objToCompare.
		* The parameter must implement IComparable 
		*/ 
		/// <summary>
		/// Evaluate a greater than or equal constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be greater than or equal to</param>
		public static AbstractConstraint GreaterThanOrEqual(IComparable objToCompare)
		{
			return new ComparingConstraint(objToCompare, true, true);
		}

	   /*
		* method: Equal
		* 
		* Determains whatever the parameter equal to obj.
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
		 * Determains whatever the parameter does not equal to obj.
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
        /// Evaluate a same as constraint.
        /// </summary>
        /// <param name="obj">The object the parameter should the same as.</param>
        public static AbstractConstraint Same(object obj)
        {
            return new Same(obj);
        }

        /// <summary>
        /// Evaluate a not same as constraint.
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
		/// A constraints that accept anything
		/// </summary>
		/// <returns></returns>
		public static AbstractConstraint Anything()
		{
			return new Anything();
		}

		/*
		 * Method: Null
		 * 
		 * Determains whatever the parameter is null
		 * 
		 */
		/// <summary>
		/// A constraint that accept only nulls
		/// </summary>
		/// <returns></returns>
		public static AbstractConstraint Null()
		{
			return new Equal(null);
		}

		/*
		 * Method: NotNull
		 * 
		 * Determains whatever the parameter is not null
		 * 
		 */
		/// <summary>
		/// A constraint that accept only non null values
		/// </summary>
		/// <returns></returns>
		public static AbstractConstraint NotNull()
		{
			return !new Equal(null);
		}

		/*
		 * Method: TypeOf
		 * 
		 * Determains whatever the parameter if of the specified type.
		 * 
		 */
		/// <summary>
		/// A constraint that accept only value of the specified type
		/// </summary>
		public static AbstractConstraint TypeOf(Type type)
		{
			return new TypeOf(type);
		}
		
		#if dotNet2
        /*
		 * Method: TypeOf
		 * 
		 * Determains whatever the parameter if of the specified type.
		 * 
		 */
        /// <summary>
        /// A constraint that accept only value of the specified type
        /// </summary>
		public static AbstractConstraint TypeOf<T>()
		{
			return new TypeOf(typeof(T));
		}
		#endif
	}
}
