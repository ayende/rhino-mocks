using Castle.Core.Interceptor;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl.Invocation.Actions
{
    ///<summary>
    ///</summary>
    public class Proceed : IInvocationActionn
    {
        ///<summary>
        ///</summary>
        public void PerformAgainst(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}