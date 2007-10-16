#region license

// Copyright (c) 2007 Ivan Krivyakov (ivan@ikriv.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
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

namespace Rhino.Mocks.Impl
{
	using System;
	using System.Reflection;
	using System.Runtime.Remoting.Messaging;
	using System.Runtime.Remoting.Proxies;
	using Castle.Core.Interceptor;
	using Interfaces;

	///<summary>
	/// This class is used to generate mocking proxies using the remoting infrastructure
	/// of .NET
	///</summary>
	public class RemotingMockGenerator
	{
		///<summary>
		/// Create the proxy using remoting
		///</summary>
		public object CreateRemotingMock(Type t, IInterceptor interceptor, IMockedObject proxyInstance)
		{
			if (t.IsInterface == false && !typeof (MarshalByRefObject).IsAssignableFrom(t))
			{
				throw new InvalidCastException(
					String.Format("Cannot create remoting proxy. '{0}' is not derived from MarshalByRefObject", t.Name));
			}

			return new InterceptionProxy(t, interceptor, proxyInstance).GetTransparentProxy();
		}

		private class RemotingInvocation : IInvocation
		{
			private readonly IMethodCallMessage _message;
			private object _returnValue;
			private readonly RealProxy _realProxy;

			public RemotingInvocation(RealProxy realProxy, IMethodCallMessage message)
			{
				_message = message;
				_realProxy = realProxy;
			}

			public object[] Arguments
			{
				get { return (object[]) _message.Properties["__Args"]; }
			}

			public Type[] GenericArguments
			{
				get
				{
					MethodBase method = _message.MethodBase;
					if (!method.IsGenericMethod)
					{
						return new Type[0];
					}

					return method.GetGenericArguments();
				}
			}

			public object GetArgumentValue(int index)
			{
				throw new NotSupportedException();
			}

			public MethodInfo GetConcreteMethod()
			{
				return (MethodInfo) _message.MethodBase;
			}

			public MethodInfo GetConcreteMethodInvocationTarget()
			{
				throw new NotSupportedException();
			}

			public object InvocationTarget
			{
				get { throw new NotSupportedException(); }
			}

			public MethodInfo Method
			{
				get { return GetConcreteMethod(); }
			}

			public MethodInfo MethodInvocationTarget
			{
				get { throw new NotSupportedException(); }
			}

			public void Proceed()
			{
				throw new InvalidOperationException("Proceed() is not applicable to remoting mocks.");
			}

			public object Proxy
			{
				get { return _realProxy.GetTransparentProxy(); }
			}

			public object ReturnValue
			{
				get { return _returnValue; }
				set { _returnValue = value; }
			}

			public void SetArgumentValue(int index, object value)
			{
				throw new NotSupportedException();
			}

			public Type TargetType
			{
				get { throw new NotSupportedException(); }
			}
		}

		internal class InterceptionProxy : RealProxy
		{
			private readonly IInterceptor _interceptor;
			private readonly IMockedObject proxyInstance;

			public InterceptionProxy(Type type, IInterceptor interceptor, IMockedObject proxyInstance)
				:
					base(type)
			{
				_interceptor = interceptor;
				this.proxyInstance = proxyInstance;
			}

			public override IMessage Invoke(IMessage msg)
			{
				IMethodCallMessage mcm = msg as IMethodCallMessage;
				if (mcm == null) return null;

				if (IsEqualsMethod(mcm))
				{
					return ReturnValue(HandleEquals(mcm), mcm);
				}

				if (IsGetHashCodeMethod(mcm))
				{
					return ReturnValue(GetHashCode(), mcm);
				}

				if (IsGetTypeMethod(mcm))
				{
					return ReturnValue(GetType(), mcm);
				}

				RemotingInvocation invocation = new RemotingInvocation(this, mcm);
				_interceptor.Intercept(invocation);

				return ReturnValue(invocation.ReturnValue, mcm);
			}

			private bool IsGetTypeMethod(IMethodCallMessage mcm)
			{
				if (mcm.MethodName != "GetType") return false;
				if (mcm.MethodBase.DeclaringType != typeof (object)) return false;
				Type[] args = mcm.MethodSignature as Type[];
				if (args == null) return false;
				return args.Length == 0;
			}

			private static bool IsEqualsMethod(IMethodMessage mcm)
			{
				if (mcm.MethodName != "Equals") return false;
				Type[] argTypes = mcm.MethodSignature as Type[];
				if (argTypes == null) return false;
				if (argTypes.Length == 1 && argTypes[0] == typeof (object)) return true;
				return false;
			}

			private static bool IsGetHashCodeMethod(IMethodMessage mcm)
			{
				if (mcm.MethodName != "GetHashCode") return false;
				Type[] argTypes = mcm.MethodSignature as Type[];
				if (argTypes == null) return false;
				return (argTypes.Length == 0);
			}


			private bool HandleEquals(IMethodMessage mcm)
			{
				object another = mcm.Args[0];
				EvilHackForPassingMockedObjectFromRemotingProxy evil = another as EvilHackForPassingMockedObjectFromRemotingProxy;
				if(evil!=null)
				{
					evil.MockedObject = proxyInstance;
					return false;
				}
				if (another == null) return false;
				return ReferenceEquals(GetTransparentProxy(), another);
			}

			private static IMessage ReturnValue(object value, IMethodCallMessage mcm)
			{
				return new ReturnMessage(value, null, 0, mcm.LogicalCallContext, mcm);
			}
		}

		internal class EvilHackForPassingMockedObjectFromRemotingProxy
		{
			public IMockedObject MockedObject;	
		}
	}
}