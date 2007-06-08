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
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks
{
	/*
	 * Class: LastCall
	 * Allows to set options for the method calls.
	 * note:
	 * If the method has a return value, it's recommended to use <Expect>
	 * 
	 */ 
	/// <summary>
	/// Allows to set various options for the last method call on
	/// a specified object.
	/// If the method has a return value, it's recommended to use Expect
	/// </summary>
	public
#if dotNet2
 static
#else
    sealed
#endif
        class LastCall
	{
		/*
		 * Method: On
		 * Gets the method options for the last call for mockedInstance.
		 * This is the recommended approach for multi threaded scenarios.
		 * 
		 * Expected usage:
		 * (start code)
		 * LastCall.On(mockObj).Return(4);
		 * (end)
		 * 
		 * Thread safety:
		 * This method is safe to use in multi threading scenarios.
		 */ 
		/// <summary>
		/// Allows to get an interface to work on the last call.
		/// </summary>
		/// <param name="mockedInstance">The mocked object</param>
		/// <returns>Interface that allows to set options for the last method call on this object</returns>
		public static IMethodOptions On(object mockedInstance)
		{
			IMockedObject mockedObj = MockRepository.GetMockedObject(mockedInstance);
			return mockedObj.Repository.LastMethodCall(mockedInstance);
		}

		/*
		 * Property: Options
		 * *Internal!*
		 * 
		 * Get the method options for the last method call from *all* the mock objects.
		 * Throws an exception if there is no such call.
		 * 
		 * Thread safety:
		 * *Not* safe for mutli threading, use <On>
		 */ 
		internal static IMethodOptions Options
		{
			get
			{
				if (MockRepository.LastMockedObject==null)
					throw new InvalidOperationException("Invalid call, the last call has been used or no call has been made (did you make a call to a non virtual method?).");
				return MockRepository.lastRepository.LastMethodCall(MockRepository.LastMockedObject);
			}
		}

		/*
		 * Method: Return
		 * 
		 * Sets the return value when the method is called.
		 * 
		 * Thread safety: 
		 * *Not* safe for mutli threading, use <On>* 
		 */ 
		/// <summary>
		/// Set the return value for the method.
		/// </summary>
		/// <param name="objToReturn">The object the method will return</param>
		/// <returns>IRepeat that defines how many times the method will return this value</returns>
		public static IMethodOptions Return(object objToReturn)
		{
			return Options.Return(objToReturn);
		}

		/*
		 * Method: Throw
		 * 
		 * Throws the specified exception when the method is called.
 		 * 
		 * Thread safety:
 		 * *Not* safe for mutli threading, use <On>		 
		 */ 
		/// <summary>
		/// Throws the specified exception when the method is called.
		/// </summary>
		/// <param name="exception">Exception to throw</param>
		public static IMethodOptions Throw(Exception exception)
		{
			return Options.Throw(exception);
		}

		/*
		 * Method: IgnoreArguments
		 * 
		 * Ignores the arguments for this method. Any arguments are considered fine for this
		 * method.
		 * 
		 * Thread safety:
 		 * *Not* safe for mutli threading, use <On>
		 */ 
		/// <summary>
		/// Ignores the arguments for this method. Any argument will be matched
		/// againt this method.
		/// </summary>
		public static IMethodOptions IgnoreArguments()
		{
			return Options.IgnoreArguments();
		}

		/*
		 * Property: Repeat
		 * 
		 * Allows to get the <Interfaces.IRepeat> instance that would allow to 
		 * set the expected number of times that this method will occur.
		 * 
		 * Thread safety:
 		 * *Not* safe for mutli threading, use <On>
		 */ 
		/// <summary>
		/// Better syntax to define repeats. 
		/// </summary>
		public static IRepeat Repeat
		{
			get { return Options.Repeat; }
		}

		/*
		 * Method: Constraints
		 * 
		 * Sets the contraints on this method parameters.
		 * The number of the constraints *must* be equal to the number of method arguments.
		 * 
		 * Thread safety:
 		 * *Not* safe for mutli threading, use <On>
		 */ 
		/// <summary>
		/// Add constraints for the method's arguments.
		/// </summary>
		public static IMethodOptions Constraints(params AbstractConstraint[] constraints)
		{
			return Options.Constraints(constraints);
		}

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
		 * Thread safety:
 		 * *Not* safe for mutli threading, use <On>
		 */ 
		/// <summary>
		/// Set a callback method for the last call
		/// </summary>
		public static IMethodOptions Callback(Delegate callback)
		{
			return Options.Callback(callback);
		}

        /// <summary>
        /// Call the original method on the class, bypassing the mocking layers, for the last call.
        /// </summary>
        [Obsolete("Use CallOriginalMethod(OriginalCallOptions options) overload to explicitly specify the call options")]
		public static void CallOriginalMethod()
        {
            Options.CallOriginalMethod();
        }

		
        /// <summary>
        /// Call the original method on the class, optionally bypassing the mocking layers, for the last call.
        /// </summary>
		public static IMethodOptions CallOriginalMethod(OriginalCallOptions options)
        {
            return Options.CallOriginalMethod(options);
        }

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
        public static IMethodOptions Do(Delegate action)
        {
            return Options.Do(action);
        }

	    /// <summary>
	    /// Gets an interface that will raise the last event when called.
	    /// </summary>
	    public static IEventRaiser GetEventRaiser()
	    {
            return Options.GetEventRaiser();
	    }
	    
	    /// <summary>
	    /// Set the parameter values for out and ref parameters.
	    /// This is done using zero based indexing, and _ignoring_ any non out/ref parameter.
	    /// </summary>
	    public static IMethodOptions OutRef(params object[] parameters)
	    {
            return Options.OutRef(parameters);
	    }
		
		/// <summary>
    	/// Documentation message for the expectation
    	/// </summary>
    	/// <param name="documentationMessage">Message</param>
		public static IMethodOptions Message(string documentationMessage)
		{
			return Options.Message(documentationMessage);
		}

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
        public static IMethodOptions PropertyBehavior()
        {
			return Options.PropertyBehavior();
        }
	}
}
