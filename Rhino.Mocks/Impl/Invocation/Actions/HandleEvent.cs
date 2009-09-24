using Castle.Core.Interceptor;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl.Invocation.Actions
{
    ///<summary>
    ///</summary>
    public class HandleEvent : IInvocationActionn
    {
        IMockedObject proxyInstance;

        ///<summary>
        ///</summary>
        public HandleEvent(IMockedObject proxy_instance)
        {
            proxyInstance = proxy_instance;
        }

        ///<summary>
        ///</summary>
        public void PerformAgainst(IInvocation invocation)
        {
            proxyInstance.HandleEvent(invocation.Method, invocation.Arguments);
        }
    }
}