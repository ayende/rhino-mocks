using System;
using System.Reflection;
using Castle.Core.Interceptor;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
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
		public void Intercept(IInvocation invocation)
		{
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
			if (proxyInstance.ShouldCallOriginal(invocation.Method))
			{
				invocation.Proceed();
				return;
			}
			if (proxyInstance.IsPropertyMethod(invocation.Method))
			{
				invocation.ReturnValue = proxyInstance.HandleProperty(invocation.Method, invocation.Arguments);
				return;
			}
			//This call handle the subscribe / remove this method call is for an event,
			//processing then continue normally (so we get an expectation for subscribing / removing from the event
			HandleEvent(invocation, invocation.Arguments);
			object proxy = repository.GetMockObjectFromInvocationProxy(invocation.Proxy);
			invocation.ReturnValue = repository.MethodCall(invocation, proxy, invocation.Method, invocation.Arguments);
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
				invocation.Method.IsSpecialName &&
				(invocation.Method.Name.StartsWith("add_") || invocation.Method.Name.StartsWith("remove_"));
		}
	}
}