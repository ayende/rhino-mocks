using System;

namespace Rhino.Mocks.Interfaces
{
	using System.Reflection;

	/// <summary>
	/// Interface to find the repository of a mocked object
	/// </summary>
	public interface IMockedObject
	{
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
        void RegisterPropertyBehaviorFor(PropertyInfo prop);

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
		void ClearState();
	}
}