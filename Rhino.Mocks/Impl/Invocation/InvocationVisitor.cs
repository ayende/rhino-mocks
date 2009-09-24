using Castle.Core.Interceptor;
using Rhino.Mocks.Impl.InvocationSpecifications;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl.Invocation
{
    ///<summary>
    ///</summary>
    public class InvocationVisitor
    {
        ISpecification<IInvocation> criteria;
        IInvocationActionn invocationAction;

        ///<summary>
        ///</summary>
        public InvocationVisitor(ISpecification<IInvocation> criteria, IInvocationActionn invocationAction)
        {
            this.criteria = criteria;
            this.invocationAction = invocationAction;
        }

        ///<summary>
        ///</summary>
        public bool CanWorkWith(IInvocation item)
        {
            return criteria.IsSatisfiedBy(item);
        }

        ///<summary>
        ///</summary>
        public void RunAgainst(IInvocation invocation)
        {
            invocationAction.PerformAgainst(invocation);
        }
    }
}