using Castle.Core.Interceptor;
using Rhino.Mocks.Impl.InvocationSpecifications;

namespace Rhino.Mocks.Impl.Invocation.Specifications
{
    ///<summary>
    ///Summary descritpion for NamedEventExistsOnDeclaringType
    ///</summary>
    public class NamedEventExistsOnDeclaringType : ISpecification<IInvocation> {

        ///<summary>
        ///</summary>
        public bool IsSatisfiedBy(IInvocation item)
        {
            return item.Method.DeclaringType.GetEvent(item.Method.Name.Substring(FollowsEventNamingStandard.AddPrefix.Length)) != null ||
                   item.Method.DeclaringType.GetEvent(item.Method.Name.Substring(FollowsEventNamingStandard.RemovePrefix.Length)) != null;
        }
    }
}