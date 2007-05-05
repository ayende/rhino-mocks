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
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
	using System.Collections;
	using System.Reflection;
	using System.Text;

	/// <summary>
	/// This is a dummy type that is used merely to give DynamicProxy the proxy instance that
	/// it needs to create IProxy's types.
	/// </summary>
	public class ProxyInstance : MarshalByRefObject, IMockedObject
	{
		private MockRepository repository;
		private int hashCode;
		private IList originalMethodsToCall;
		private IList propertiesToSimulate;
		private IDictionary propertiesValues;
		private IDictionary eventsSubscribers;
		private Type[] implemented;

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
		/// </summary>
		public void RegisterPropertyBehaviorFor(PropertyInfo prop)
		{
			if (propertiesToSimulate == null)
				propertiesToSimulate = new ArrayList();
			propertiesToSimulate.Add(prop.GetGetMethod());
			propertiesToSimulate.Add(prop.GetSetMethod());
		}

		/// <summary>
		/// Check if the method was registered as a property method.
		/// </summary>
		public bool IsPropertyMethod(MethodInfo method)
		{
			if (propertiesToSimulate == null)
				return false;
			return propertiesToSimulate.Contains(method);
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
						string.Format("Can't return a value for property {0} because no value was set and the Property return a value type.", method.Name.Substring(4)));
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

			Delegate subscriber = (Delegate)args[0];
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
			return (Delegate)eventsSubscribers[eventName];
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
		/// Gets the implemented types by this mocked object
		/// </summary>
		/// <value>The implemented.</value>
		public Type[] ImplementedTypes
		{
			get { return implemented; }
		}

		/// <summary>
		/// Clears the state of the object, remove original calls, property behavior, subscribed events, etc.
		/// </summary>
		public void ClearState()
		{
			if (eventsSubscribers != null)
				eventsSubscribers.Clear();
			if (originalMethodsToCall != null)
				originalMethodsToCall.Clear();
			if (propertiesValues != null)
				propertiesValues.Clear();
			if (propertiesToSimulate != null)
				propertiesToSimulate.Clear();
		}

		private static string GenerateKey(MethodInfo method, object[] args)
		{
			if ((method.Name.StartsWith("get_") && args.Length == 0) ||
				(method.Name.StartsWith("set_") && args.Length == 1))
				return method.Name.Substring(4);
			StringBuilder sb = new StringBuilder();
			sb.Append(method.Name.Substring(4));
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
			Delegate existing = (MulticastDelegate)eventsSubscribers[eventName];
			existing = MulticastDelegate.Remove(existing, subscriber);
			eventsSubscribers[eventName] = existing;
		}

		private void AddEvent(MethodInfo method, Delegate subscriber)
		{
			string eventName = method.Name.Substring(4);
			Delegate existing = (MulticastDelegate)eventsSubscribers[eventName];
			existing = MulticastDelegate.Combine(existing, subscriber);
			eventsSubscribers[eventName] = existing;
		}
	}
}
