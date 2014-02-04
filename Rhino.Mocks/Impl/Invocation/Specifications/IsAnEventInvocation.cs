using Castle.DynamicProxy;
using Rhino.Mocks.Impl.InvocationSpecifications;

namespace Rhino.Mocks.Impl.Invocation.Specifications
{
    ///<summary>
    ///</summary>
    public class IsAnEventInvocation : ISpecification<IInvocation>
    {
        ///<summary>
        ///</summary>
        public bool IsSatisfiedBy(IInvocation item)
        {
            return new AndSpecification<IInvocation>(new FollowsEventNamingStandard(),
                                                     new NamedEventExistsOnDeclaringType()).IsSatisfiedBy(item);
        }
    }

    ///<summary>
    ///</summary>
    public class AnyInvocation : ISpecification<IInvocation>
    {
        ///<summary>
        ///</summary>
        public bool IsSatisfiedBy(IInvocation item)
        {
            return true;
        }
    }
}