using System;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Constraints
{
    /*
     * Class: PublicField
     * 
     * Constraints for dealing with object's public fields
     */
    /// <summary>
    /// Central location for constraints for object's public fields
	/// </summary>
	public static class PublicField
	{
        /*
         * Method: Value
         * 
         * Determines that the parameter has a public field with the specified value
         * 
         */

        /// <summary>
		/// Constrains the parameter to have a public field with the specified value
		/// </summary>
        /// <param name="publicFieldName">Name of the public field.</param>
		/// <param name="expectedValue">Expected value.</param>
		/// <returns></returns>
		public static AbstractConstraint Value(string publicFieldName, object expectedValue)
		{
            return new PublicFieldIs(publicFieldName, expectedValue);
		}

        /// <summary>
        /// Constrains the parameter to have a public field with the specified value.
        /// </summary>
        /// <param name="declaringType">The type that declares the public field, used to disambiguate between public fields.</param>
        /// <param name="publicFieldName">Name of the public field.</param>
        /// <param name="expectedValue">Expected value.</param>
        /// <returns></returns>
        public static AbstractConstraint Value(Type declaringType, string publicFieldName, object expectedValue)
        {
            return new PublicFieldIs(declaringType, publicFieldName, expectedValue);
        }

        /// <summary>
        /// Constrains the parameter to have a public field satisfying a specified constraint.
        /// </summary>
        /// <param name="publicFieldName">Name of the public field.</param>
        /// <param name="publicFieldConstraint">Constraint for the public field.</param>
        public static AbstractConstraint ValueConstraint(string publicFieldName, AbstractConstraint publicFieldConstraint)
        {
            return new PublicFieldConstraint(publicFieldName, publicFieldConstraint);
        }

        /// <summary>
        /// Constrains the parameter to have a public field satisfying a specified constraint.
        /// </summary>
        /// <param name="declaringType">The type that declares the public field, used to disambiguate between public fields.</param>
        /// <param name="publicFieldName">Name of the public field.</param>
        /// <param name="publicFieldConstraint">Constraint for the public field.</param>
        public static AbstractConstraint ValueConstraint(Type declaringType, string publicFieldName, AbstractConstraint publicFieldConstraint)
        {
            return new PublicFieldConstraint(declaringType, publicFieldName, publicFieldConstraint);
        }

        /*
         * Method: IsNull
         * 
         * Determines that the parameter has a public field with null value
         * 
         */
        /// <summary>
        /// Determines whether the parameter has the specified public field and that it is null.
		/// </summary>
        /// <param name="publicFieldName">Name of the public field.</param>
		/// <returns></returns>
        public static AbstractConstraint IsNull(string publicFieldName)
		{
            return new PublicFieldIs(publicFieldName, null);
		}

        /// <summary>
        /// Determines whether the parameter has the specified public field and that it is null.
        /// </summary>
        /// <param name="declaringType">The type that declares the public field, used to disambiguate between public fields.</param>
        /// <param name="publicFieldName">Name of the public field.</param>
        /// <returns></returns>
        public static AbstractConstraint IsNull(Type declaringType, string publicFieldName)
        {
            return new PublicFieldIs(declaringType, publicFieldName, null);
        }

        /*
         * Method: IsNotNull
         * 
         * Determines that the parameter has a public field with non-null value
         * 
         */
        /// <summary>
        /// Determines whether the parameter has the specified public field and that it is not null.
		/// </summary>
        /// <param name="publicFieldName">Name of the public field.</param>
		/// <returns></returns>
        public static AbstractConstraint IsNotNull(string publicFieldName)
		{
            return !new PublicFieldIs(publicFieldName, null);
		}

        /// <summary>
        /// Determines whether the parameter has the specified public field and that it is not null.
        /// </summary>
        /// <param name="declaringType">The type that declares the public field, used to disambiguate between public fields.</param>
        /// <param name="publicFieldName">Name of the public field.</param>
        /// <returns></returns>
        public static AbstractConstraint IsNotNull(Type declaringType, string publicFieldName)
        {
            return !new PublicFieldIs(declaringType, publicFieldName, null);
        }
	}
}
