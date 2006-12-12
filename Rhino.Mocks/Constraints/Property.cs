using System;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks
{
	/*
	 * Class: Property
	 * 
	 * Constraints for dealing with object's properties
	 */ 
	/// <summary>
	/// Central location for constraints for object's properties
	/// </summary>
	public
#if dotNet2
 static
#else
    sealed
#endif
        class Property
	{
		/*
		 * Method: Value
		 * 
		 * Determains that the parameter has property with the specified value
		 * 
		 */ 

        /// <summary>
		/// Constrains the parameter to have property with the specified value
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="expectedValue">Expected value.</param>
		/// <returns></returns>
		public static AbstractConstraint Value(string propertyName, object expectedValue)
		{
			return new PropertyIs(propertyName, expectedValue);
		}

        /// <summary>
        /// Constrains the parameter to have property with the specified value.
        /// </summary>
        /// <param name="declaringType">The type that declares the property, used to disambiguate between properties.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="expectedValue">Expected value.</param>
        /// <returns></returns>
        public static AbstractConstraint Value(Type declaringType, string propertyName, object expectedValue)
        {
            return new PropertyIs(declaringType, propertyName, expectedValue);
        }

        /// <summary>
        /// Constrains the parameter to have a property satisfying a specified constraint.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyConstraint">Constraint for the property.</param>
        public static AbstractConstraint ValueConstraint(string propertyName, AbstractConstraint propertyConstraint)
        {
            return new PropertyConstraint(propertyName, propertyConstraint);
        }

        /// <summary>
        /// Constrains the parameter to have a property satisfying a specified constraint.
        /// </summary>
        /// <param name="declaringType">The type that declares the property, used to disambiguate between properties.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyConstraint">Constraint for the property.</param>
        public static AbstractConstraint ValueConstraint(Type declaringType, string propertyName, AbstractConstraint propertyConstraint)
        {
            return new PropertyConstraint(declaringType, propertyName, propertyConstraint);
        }

		/*
		 * Method: IsNull
		 * 
		 * Determains that the parameter has property with null value
		 * 
		 */ 
		/// <summary>
		/// Determines whether the parameter has the specified property and that it is null.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns></returns>
		public static AbstractConstraint IsNull(string propertyName)
		{
			return new PropertyIs(propertyName, null);
		}

        /// <summary>
        /// Determines whether the parameter has the specified property and that it is null.
        /// </summary>
        /// <param name="declaringType">The type that declares the property, used to disambiguate between properties.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static AbstractConstraint IsNull(Type declaringType, string propertyName)
        {
            return new PropertyIs(declaringType, propertyName, null);
        }

		/*
		 * Method: IsNotNull
		 * 
		 * Determains that the parameter has property with non-null value
		 * 
		 */ 
		/// <summary>
		/// Determines whether the parameter has the specified property and that it is not null.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns></returns>
		public static AbstractConstraint IsNotNull(string propertyName)
		{
			return !new PropertyIs(propertyName, null);
		}

        /// <summary>
        /// Determines whether the parameter has the specified property and that it is not null.
        /// </summary>
        /// <param name="declaringType">The type that declares the property, used to disambiguate between properties.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static AbstractConstraint IsNotNull(Type declaringType, string propertyName)
        {
            return !new PropertyIs(declaringType, propertyName, null);
        }
	}
}
