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
