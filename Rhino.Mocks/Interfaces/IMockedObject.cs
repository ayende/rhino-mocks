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
using System.Collections.Generic;

namespace Rhino.Mocks.Interfaces
{
	using System.Reflection;

	/// <summary>
	/// Interface to find the repository of a mocked object
	/// </summary>
	public interface IMockedObject
	{
        /// <summary>
        /// Mocks that are tied to this mock lifestyle
        /// </summary>
        IList<IMockedObject> DependentMocks { get; }

		/// <summary>
		/// The unique hash code of this mock, which is not related
		/// to the value of the GetHashCode() call on the object.
		/// </summary>
		int ProxyHash { get; }

		/// <summary>
		/// Gets the repository.
		/// </summary>
		MockRepository Repository { get; }

		/// <summary>
		/// Return true if it should call the original method on the object
		/// instead of pass it to the message chain.
		/// </summary>
		/// <param name="method">The method to call</param>
		bool ShouldCallOriginal(MethodInfo method);

		/// <summary>
		/// Register a method to be called on the object directly
		/// </summary>
		void RegisterMethodForCallingOriginal(MethodInfo method);

        /// <summary>
        /// Register a property on the object that will behave as a simple property
        /// </summary>
        bool RegisterPropertyBehaviorFor(PropertyInfo prop);

        /// <summary>
        /// Check if the method was registered as a property method.
        /// </summary>
        bool IsPropertyMethod(MethodInfo method);

        /// <summary>
        /// Do get/set on the property, according to need.
        /// </summary>
        object HandleProperty(MethodInfo method, object[] args);

        /// <summary>
        /// Do add/remove on the event
        /// </summary>
        void HandleEvent(MethodInfo method, object[] args);

        /// <summary>
        /// Get the subscribers of a spesific event
        /// </summary>
        Delegate GetEventSubscribers(string eventName);


		/// <summary>
		/// Gets the declaring type of the method, taking into acccount the possible generic 
		/// parameters that it was created with.
		/// </summary>
		Type GetDeclaringType(MethodInfo info);

		/// <summary>
		/// Clears the state of the object, remove original calls, property behavior, subscribed events, etc.
		/// </summary>
		void ClearState(BackToRecordOptions options);

		/// <summary>
		/// Gets the implemented types by this mocked object
		/// </summary>
		/// <value>The implemented.</value>
		Type[] ImplementedTypes { get; }

		/// <summary>
		/// Gets or sets the constructor arguments.
		/// </summary>
		/// <value>The constructor arguments.</value>
	    object[] ConstructorArguments { get; set; }

        /// <summary>
        /// The mocked instance that this is representing
        /// </summary>
	    object MockedObjectInstance { get; set; }

	    /// <summary>
        /// Get all the method calls arguments that were made against this object with the specificed 
        /// method.
        /// </summary>
        /// <remarks>
        /// Only method calls in replay mode are counted
        /// </remarks>
	    ICollection<object[]> GetCallArgumentsFor(MethodInfo method);

        /// <summary>
        /// Records the method call
        /// </summary>
	    void MethodCall(MethodInfo method, object[] args);
	}
}