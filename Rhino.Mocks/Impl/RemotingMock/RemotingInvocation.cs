using System;

namespace Rhino.Mocks.Impl.RemotingMock
{
    using System;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;
    using Castle.DynamicProxy;

    /// <summary>
    /// Implementation of IInvocation based on remoting proxy
    /// </summary>
    /// <remarks>Some methods are marked NotSupported since they either don't make sense
    /// for remoting proxies, or they are never called by Rhino Mocks</remarks>
    internal class RemotingInvocation : IInvocation
    {
        private readonly IMethodCallMessage _message;
        private object _returnValue;
        private readonly RealProxy _realProxy;
		private object[] _args; 

        public RemotingInvocation(RealProxy realProxy, IMethodCallMessage message)
        {
            _message = message;
            _realProxy = realProxy;
			this._args = (object[])this._message.Properties["__Args"]; 
        }

        public object[] Arguments
        {
            get { return _args; }
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
            return (MethodInfo)_message.MethodBase;
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
}
