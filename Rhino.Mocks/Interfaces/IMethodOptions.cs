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
using System.Reflection;

namespace Rhino.Mocks.Interfaces
{
	/*
	 * Interface: IMethodOptions
	 * 
	 * Allows to define what would happen when a method is called.
	 * 
	 */ 
	/// <summary>
	/// Allows to define what would happen when a method 
	/// is called.
	/// </summary>
	public interface IMethodOptions
	{
		/*
		 * Method: Return
		 * 
		 * Sets the return value when the method is called.
		 */ 
		/// <summary>
		/// Set the return value for the method.
		/// </summary>
		/// <param name="objToReturn">The object the method will return</param>
		/// <returns>IRepeat that defines how many times the method will return this value</returns>
		IMethodOptions Return(object objToReturn);

		/*
		 * Method: Throw
		 * 
		 * Throws the specified exception when the method is called.
		 */
		/// <summary>
		/// Throws the specified exception when the method is called.
		/// </summary>
		/// <param name="exception">Exception to throw</param>
		IMethodOptions Throw(Exception exception);

		/*
		 * Method: IgnoreArguments
		 * 
		 * Ignores the arguments for this method. Any arguments are considered fine for this
		 * method.
		 */
		/// <summary>
		/// Ignores the arguments for this method. Any argument will be matched
		/// againt this method.
		/// </summary>
		IMethodOptions IgnoreArguments();

		/*
		 * Property: Repeat
		 * 
		 * Allows to get the <Interfaces.IRepeat> instance that would allow to 
		 * set the expected number of times that this method will occur.
		 */ 
		/// <summary>
		/// Better syntax to define repeats. 
		/// </summary>
		IRepeat Repeat { get; }

		/*
		 * Method: Constraints
		 * 
		 * Sets the contraints on this method parameters.
		 * The number of the constraints *must* be equal to the number of method arguments.
		 */
		/// <summary>
		/// Add constraints for the method's arguments.
		/// </summary>
		IMethodOptions Constraints(params AbstractConstraint[] constraints);

		/*
		 * Method: Callback
		 * 
		 * Sets a callback delegate to be called when this method is called.
		 * 
		 * Important:
		 * The callback *must* have the same signature as the last method call but its return value
		 * *must* be a boolean.
		 * The callback will be called with the same parameters as the method and the method will
		 * be accepted if the delegate return a positive value.
		 * Note:
		 * The callback may be called several times
		 * 
		 */
		/// <summary>
		/// Set a callback method for the last call
		/// </summary>
		IMethodOptions Callback(Delegate callback);

        /*
         * Method: Do
         * 
         * Set an action to run when the expectation is matched.
         * 
         * Important:
         * The action's delegate *must* have the same signature as the last methdo call, and its return
         * value must be assignable to the last method call return value.
         * 
         * Note:
         * This method is only called once, after the method call was match to the expectation.
         * 
         * 
         */ 
        /// <summary>
        /// Set a delegate to be called when the expectation is matched.
        /// The delegate return value will be returned from the expectation.
        /// </summary>
        IMethodOptions Do(Delegate action);

		/*
			 * Method: CallOriginalMethod
			 * 
			 * Call the original method on the class, bypassing the mocking layers.
			 * 
			 * Important:
			 * Can only be used on a method that has an implementation. 
			 * If you try that on an interface method or an abstract method, you'll get an 
			 * exception.
			 * 
			 */ 
		/// <summary>
		/// Call the original method on the class, bypassing the mocking layers.
		/// </summary>
		/// <returns></returns>
		[Obsolete("Use CallOriginalMethod(OriginalCallOptions options) overload to explicitly specify the call options")]
		void CallOriginalMethod();

		/*
		 * Method: CallOriginalMethod
		 * 
		 * Call the original method on the class, optionally bypassing the mocking layers.
		 * 
		 * Important:
		 * Can only be used on a method that has an implementation. 
		 * If you try that on an interface method or an abstract method, you'll get an 
		 * exception.
		 * 
		 */ 
		/// <summary>
		/// Call the original method on the class, optionally bypassing the mocking layers.
		/// </summary>
		/// <returns></returns>
		IMethodOptions CallOriginalMethod(OriginalCallOptions options);

        /* Method: PropertyBehavior
         * 
         * Use the property as a normal property, so you can use it to save/load values
         * without having to specify expectations for it.
         * 
         * Note:
         * This can be called only when the last call is a getter or setter.
         */ 
        /// <summary>
        /// Use the property as a simple property, getting/setting the values without
        /// causing mock expectations.
        /// </summary>
        IMethodOptions PropertyBehavior();

        /// <summary>
        /// Get an event raiser for the last subscribed event.
        /// </summary>
	    IEventRaiser GetEventRaiser();

        /// <summary>
        /// Set the parameter values for out and ref parameters.
        /// This is done using zero based indexing, and _ignoring_ any non out/ref parameter.
        /// </summary>
        IMethodOptions OutRef(params object[] parameters);
		
		/// <summary>
    	/// Documentation message for the expectation
    	/// </summary>
    	/// <param name="documentationMessage">Message</param>
		IMethodOptions Message(string documentationMessage);
	}
}