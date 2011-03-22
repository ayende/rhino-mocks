using Castle.DynamicProxy;
using Rhino.Mocks.Impl.InvocationSpecifications;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl.Invocation.Specifications
{
    ///<summary>
    ///</summary>
    public class IsInvocationThatShouldTargetOriginal : ISpecification<IInvocation>
    {
        readonly IMockedObject proxyInstance;

        ///<summary>
        ///</summary>
        public IsInvocationThatShouldTargetOriginal(IMockedObject proxyInstance)
        {
            this.proxyInstance = proxyInstance;
        }

        ///<summary>
        ///</summary>
        public bool IsSatisfiedBy(IInvocation item)
        {
            return proxyInstance.ShouldCallOriginal(item.GetConcreteMethod());
        }
    }
}