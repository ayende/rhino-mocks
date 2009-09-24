using Castle.Core.Interceptor;
using Rhino.Mocks.Impl.InvocationSpecifications;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl.Invocation.Specifications
{
    ///<summary>
    ///</summary>
    public class IsAPropertyInvocation : ISpecification<IInvocation>
    {
        IMockedObject proxyInstance;

        ///<summary>
        ///</summary>
        public IsAPropertyInvocation(IMockedObject proxy_instance)
        {
            proxyInstance = proxy_instance;
        }

        ///<summary>
        ///</summary>
        public bool IsSatisfiedBy(IInvocation item)
        {
            return proxyInstance.IsPropertyMethod(item.GetConcreteMethod());
        }
    }
}