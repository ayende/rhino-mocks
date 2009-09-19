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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// This is a dummy type that is used merely to give DynamicProxy the proxy instance that
	/// it needs to create IProxy's types.
	/// </summary>
	public class ProxyInstance : MarshalByRefObject, IMockedObject
	{
		private readonly MockRepository repository;
		private readonly int hashCode;
		private IList originalMethodsToCall;
		private IList propertiesToSimulate;
		private IDictionary propertiesValues;
		private IDictionary eventsSubscribers;
		private readonly Type[] implemented;

	    private readonly IDictionary<MethodInfo, ICollection<object[]>> methodToActualCalls = new Dictionary<MethodInfo, ICollection<object[]>>();
	    private object[] constructorArguments = new object[0];
	    private IList<IMockedObject> dependentMocks = new List<IMockedObject>();

	    /// <summary>
		/// Create a new instance of <see cref="ProxyInstance"/>
		/// </summary>
		public ProxyInstance(MockRepository repository, params Type[] implemented)
		{
			this.repository = repository;
			this.implemented = implemented;
			hashCode = MockedObjectsEquality.NextHashCode;
		}

        /// <summary>
        /// Mocks that are tied to this mock lifestyle
        /// </summary>
        public IList<IMockedObject> DependentMocks
	    {
            get { return dependentMocks; }
	    }

	    /// <summary>
		/// The unique hash code of this proxy, which is not related
		/// to the value of the GetHashCode() call on the object.
		/// </summary>
		public int ProxyHash
		{
			get { return hashCode; }
		}

		/// <summary>
		/// Gets the repository.
		/// </summary>
		public MockRepository Repository
		{
			get { return repository; }
		}

		/// <summary>
		/// Return true if it should call the original method on the object
		/// instead of pass it to the message chain.
		/// </summary>
		/// <param name="method">The method to call</param>
		public bool ShouldCallOriginal(MethodInfo method)
		{
			if (originalMethodsToCall == null)
				return false;
			return originalMethodsToCall.Contains(method);
		}

		/// <summary>
		/// Register a method to be called on the object directly
		/// </summary>
		public void RegisterMethodForCallingOriginal(MethodInfo method)
		{
			if (originalMethodsToCall == null)
				originalMethodsToCall = new ArrayList();
			originalMethodsToCall.Add(method);
		}

		/// <summary>
		/// Register a property on the object that will behave as a simple property
		/// Return true if there is already a value for the property
		/// </summary>
		public bool RegisterPropertyBehaviorFor(PropertyInfo prop)
		{
			if (propertiesToSimulate == null)
				propertiesToSimulate = new ArrayList();
			MethodInfo getMethod = prop.GetGetMethod(true);
            MethodInfo setMethod = prop.GetSetMethod(true);
			if (propertiesToSimulate.Contains(getMethod) == false)
				propertiesToSimulate.Add(getMethod);
			if (propertiesToSimulate.Contains(setMethod) == false)
				propertiesToSimulate.Add(setMethod);
			return propertiesValues!=null &&
				propertiesValues.Contains(GenerateKey(getMethod, new object[0]));
		}

		/// <summary>
		/// Check if the method was registered as a property method.
		/// </summary>
		public bool IsPropertyMethod(MethodInfo method)
		{
			if (propertiesToSimulate == null)
				return false;
			//we have to do it this way, to handle generic types
			foreach (MethodInfo info in propertiesToSimulate)
			{
				if( AreMethodEquals(info, method))
					return true;
			}
			return false;
		}

		private static bool AreMethodEquals(MethodInfo left, MethodInfo right)
		{
			if (left.Equals(right))
				return true;
			// GetHashCode calls to RuntimeMethodHandle.StripMethodInstantiation()
			// which is needed to fix issues with method equality from generic types.
			if (left.GetHashCode() != right.GetHashCode())
				return false;
			if (left.DeclaringType != right.DeclaringType)
				return false;
			ParameterInfo[] leftParams = left.GetParameters();
			ParameterInfo[] rightParams = right.GetParameters();
			if (leftParams.Length != rightParams.Length)
				return false;
			for (int i = 0; i < leftParams.Length; i++)
			{
				if (leftParams[i].ParameterType != rightParams[i].ParameterType)
					return false;
			}
			if (left.ReturnType != right.ReturnType)
				return false;
			return true;
		}

		/// <summary>
		/// Do get/set on the property, according to need.
		/// </summary>
		public object HandleProperty(MethodInfo method, object[] args)
		{
			if (propertiesValues == null)
				propertiesValues = new Hashtable();
		    
            if (method.Name.StartsWith("get_"))
			{
				string key = GenerateKey(method, args);
				if (propertiesValues.Contains(key) == false && method.ReturnType.IsValueType)
				{
					throw new InvalidOperationException(
						string.Format(
							"Can't return a value for property {0} because no value was set and the Property return a value type.",
							method.Name.Substring(4)));
				}
				return propertiesValues[key];
			}

			object value = args[args.Length - 1];
			propertiesValues[GenerateKey(method, args)] = value;
			return null;
		}


		/// <summary>
		/// Do add/remove on the event
		/// </summary>
		public void HandleEvent(MethodInfo method, object[] args)
		{
			if (eventsSubscribers == null)
				eventsSubscribers = new Hashtable();

			Delegate subscriber = (Delegate) args[0];
			if (method.Name.StartsWith("add_"))
			{
				AddEvent(method, subscriber);
			}
			else
			{
				RemoveEvent(method, subscriber);
			}
		}

		/// <summary>
		/// Get the subscribers of a spesific event
		/// </summary>
		public Delegate GetEventSubscribers(string eventName)
		{
			if (eventsSubscribers == null)
				return null;
			return (Delegate) eventsSubscribers[eventName];
		}

		/// <summary>
		/// Gets the declaring type of the method, taking into acccount the possible generic 
		/// parameters that it was created with.
		/// </summary>
		public Type GetDeclaringType(MethodInfo info)
		{
			Type typeDeclaringTheMethod = info.DeclaringType;
			foreach (Type type in implemented)
			{
				if (type == typeDeclaringTheMethod)
					return type;
				if (typeDeclaringTheMethod.IsGenericType &&
				    typeDeclaringTheMethod == type.GetGenericTypeDefinition())
					return type;
			}
			return null;
		}


		/// <summary>
		/// Gets or sets the constructor arguments.
		/// </summary>
		/// <value>The constructor arguments.</value>
	    public object[] ConstructorArguments
	    {
	        get { return constructorArguments; }
	        set { constructorArguments = value; }
	    }

		private object mockedObjectInstance;

        /// <summary>
        /// The mocked instance that this is representing
        /// </summary>
        public object MockedObjectInstance
        {
            get { return mockedObjectInstance; } 
			set { mockedObjectInstance = value; }
        }

	    /// <summary>
		/// Gets the implemented types by this mocked object
		/// </summary>
		/// <value>The implemented.</value>
		public Type[] ImplementedTypes
		{
			get { return implemented; }
		}

		/// <summary>
		/// Get all the method calls arguments that were made against this object with the specificed
		/// method.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		/// <remarks>
		/// Only method calls in replay mode are counted
		/// </remarks>
	    public ICollection<object[]> GetCallArgumentsFor(MethodInfo method)
	    {
            if (methodToActualCalls.ContainsKey(method) == false)
                return new List<object[]>();
	        return methodToActualCalls[method];
	    }


		/// <summary>
		/// Records the method call
		/// </summary>
		/// <param name="method"></param>
		/// <param name="args"></param>
	    public void MethodCall(MethodInfo method, object[] args)
	    {
	        if(repository.IsInReplayMode(this)==false)
	            return;
            if (methodToActualCalls.ContainsKey(method) == false)
                methodToActualCalls[method] = new List<object[]>();
	        methodToActualCalls[method].Add(args);
        }

	    /// <summary>
		/// Clears the state of the object, remove original calls, property behavior, subscribed events, etc.
		/// </summary>
		public void ClearState(BackToRecordOptions options)
		{
			if (eventsSubscribers != null &&
			    (options & BackToRecordOptions.EventSubscribers) != 0)
				eventsSubscribers.Clear();
			if (originalMethodsToCall != null &&
			    (options & BackToRecordOptions.OriginalMethodsToCall) != 0)
				originalMethodsToCall.Clear();
			if (propertiesValues != null &&
			    (options & BackToRecordOptions.PropertyBehavior) != 0)
				propertiesValues.Clear();
			if (propertiesToSimulate != null &&
			    (options & BackToRecordOptions.PropertyBehavior) != 0)
				propertiesToSimulate.Clear();
		}

		private static string GenerateKey(MethodInfo method, object[] args)
		{
            var baseName = method.DeclaringType.FullName + method.Name.Substring(4); 
            if ((method.Name.StartsWith("get_") && args.Length == 0) ||
			    (method.Name.StartsWith("set_") && args.Length == 1))
				return baseName;
			StringBuilder sb = new StringBuilder();
            sb.Append(baseName);
			int len = args.Length;
			if (method.Name.StartsWith("set_"))
				len--;
			for (int i = 0; i < len; i++)
			{
				sb.Append(args[i].GetHashCode());
			}
			return sb.ToString();
		}

		private void RemoveEvent(MethodInfo method, Delegate subscriber)
		{
			string eventName = method.Name.Substring(7);
			Delegate existing = (MulticastDelegate) eventsSubscribers[eventName];
			existing = MulticastDelegate.Remove(existing, subscriber);
			eventsSubscribers[eventName] = existing;
		}

		private void AddEvent(MethodInfo method, Delegate subscriber)
		{
			string eventName = method.Name.Substring(4);
			Delegate existing = (MulticastDelegate) eventsSubscribers[eventName];
			existing = MulticastDelegate.Combine(existing, subscriber);
			eventsSubscribers[eventName] = existing;
		}
	}
}