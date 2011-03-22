namespace Rhino.Mocks.Impl.RemotingMock
{
    using System;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;
    using Castle.DynamicProxy;
    using Rhino.Mocks.Interfaces;

    internal class RemotingProxy : RealProxy
    {
        private readonly IInterceptor _interceptor;
        private readonly IMockedObject _mockedObject;

        public RemotingProxy(Type type, IInterceptor interceptor, IMockedObject mockedObject)
            :
                base(type)
        {
            _interceptor = interceptor;
            _mockedObject = mockedObject;
        }

        public IMockedObject MockedObject
        {
            get { return _mockedObject; }
        }

		private static IMessage ReturnValue(object value, object[] outParams, IMethodCallMessage mcm)
		{
			return new ReturnMessage(value, outParams, outParams == null ? 0 : outParams.Length, mcm.LogicalCallContext, mcm);
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
                return ReturnValue(GetProxiedType(), mcm);
            }

            if (IsToStringMethod(mcm))
            {
                string retVal = String.Format("RemotingMock_{1}<{0}>", this.GetProxiedType().Name, this.GetHashCode());
                return ReturnValue(retVal, mcm);
            }

            RemotingInvocation invocation = new RemotingInvocation(this, mcm);
            _interceptor.Intercept(invocation);

			return ReturnValue(invocation.ReturnValue, invocation.Arguments, mcm);
        }

        private bool IsGetTypeMethod(IMethodCallMessage mcm)
        {
            if (mcm.MethodName != "GetType") return false;
            if (mcm.MethodBase.DeclaringType != typeof(object)) return false;
            Type[] args = mcm.MethodSignature as Type[];
            if (args == null) return false;
            return args.Length == 0;
        }

        private static bool IsEqualsMethod(IMethodMessage mcm)
        {
            if (mcm.MethodName != "Equals") return false;
            Type[] argTypes = mcm.MethodSignature as Type[];
            if (argTypes == null) return false;
            if (argTypes.Length == 1 && argTypes[0] == typeof(object)) return true;
            return false;
        }

        private static bool IsGetHashCodeMethod(IMethodMessage mcm)
        {
            if (mcm.MethodName != "GetHashCode") return false;
            Type[] argTypes = mcm.MethodSignature as Type[];
            if (argTypes == null) return false;
            return (argTypes.Length == 0);
        }

        private static bool IsToStringMethod(IMethodCallMessage mcm)
        {
            if (mcm.MethodName != "ToString") return false;
            Type[] args = mcm.MethodSignature as Type[];
            if (args == null) return false;
            return args.Length == 0;
        }


        private bool HandleEquals(IMethodMessage mcm)
        {
            object another = mcm.Args[0];
            if (another == null) return false;

            if (another is IRemotingProxyOperation)
            {
                ((IRemotingProxyOperation)another).Process(this);
                return false;
            }

            return ReferenceEquals(GetTransparentProxy(), another);
        }

        private static IMessage ReturnValue(object value, IMethodCallMessage mcm)
        {
            return new ReturnMessage(value, null, 0, mcm.LogicalCallContext, mcm);
        }
    }
}
