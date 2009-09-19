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
using System.Reflection;
using Castle.Core.Interceptor;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
	using System.Runtime.CompilerServices;

	/// <summary>
	/// Summary description for RhinoInterceptor.
	/// </summary>
	public class RhinoInterceptor : MarshalByRefObject, IInterceptor
	{
		private readonly MockRepository repository;
		private readonly IMockedObject proxyInstance;

		private static MethodInfo[] objectMethods =
			new MethodInfo[]
				{
					typeof (object).GetMethod("ToString"), typeof (object).GetMethod("Equals", new Type[] {typeof (object)}),
					typeof (object).GetMethod("GetHashCode"), typeof (object).GetMethod("GetType")
				};

		/// <summary>
		/// Creates a new <see cref="RhinoInterceptor"/> instance.
		/// </summary>
		public RhinoInterceptor(MockRepository repository, IMockedObject proxyInstance)
		{
			this.repository = repository;
			this.proxyInstance = proxyInstance;
		}

		/// <summary>
		/// Intercept a method call and direct it to the repository.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Intercept(IInvocation invocation)
		{
		    proxyInstance.MockedObjectInstance = invocation.Proxy;
			if (Array.IndexOf(objectMethods, invocation.Method) != -1)
			{
				invocation.Proceed();
				return;
			}
			if (invocation.Method.DeclaringType == typeof (IMockedObject))
			{
				invocation.ReturnValue = invocation.Method.Invoke(proxyInstance, invocation.Arguments);
				return;
			}
			if (proxyInstance.ShouldCallOriginal(invocation.GetConcreteMethod()))
			{
				invocation.Proceed();
				return;
			}
			if (proxyInstance.IsPropertyMethod(invocation.GetConcreteMethod()))
			{
				invocation.ReturnValue = proxyInstance.HandleProperty(invocation.GetConcreteMethod(), invocation.Arguments);
			    repository.RegisterPropertyBehaviorOn(proxyInstance);
				return;
			}
			//This call handle the subscribe / remove this method call is for an event,
			//processing then continue normally (so we get an expectation for subscribing / removing from the event
			HandleEvent(invocation, invocation.Arguments);
			object proxy = repository.GetMockObjectFromInvocationProxy(invocation.Proxy);
			MethodInfo method = invocation.GetConcreteMethod();
			invocation.ReturnValue = repository.MethodCall(invocation, proxy, method, invocation.Arguments);
		}

		private void HandleEvent(IInvocation invocation, object[] args)
		{
			if (IsEvent(invocation))
			{
				proxyInstance.HandleEvent(invocation.Method, args);
			}
		}

		private bool IsEvent(IInvocation invocation)
		{
			return
                invocation.Method.Name.StartsWith("add_") || 
                invocation.Method.Name.StartsWith("remove_");
		}
	}
}