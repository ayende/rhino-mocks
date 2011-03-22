using Castle.DynamicProxy;
using Rhino.Mocks.Impl.InvocationSpecifications;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl.Invocation.Specifications
{
    ///<summary>
    ///</summary>
    public class IsAnInvocationOnAMockedObject : ISpecification<IInvocation>
    {
        ///<summary>
        ///</summary>
        public bool IsSatisfiedBy(IInvocation invocation)
        {
            return invocation.Method.DeclaringType == typeof (IMockedObject);
        }
    }
}