using Castle.DynamicProxy;
using Rhino.Mocks.Impl.InvocationSpecifications;

namespace Rhino.Mocks.Impl.Invocation.Specifications
{
    ///<summary>
    ///Summary description for FollowsEventNamingStandard
    ///</summary>
    public class FollowsEventNamingStandard : ISpecification<IInvocation> {
        ///<summary>
        ///</summary>
        public const string AddPrefix = "add_";
        ///<summary>
        ///</summary>
        public const string RemovePrefix = "remove_";

        ///<summary>
        ///</summary>
        public bool IsSatisfiedBy(IInvocation item)
        {
            return item.Method.Name.StartsWith(AddPrefix) ||
                   item.Method.Name.StartsWith(RemovePrefix);
        }
    }
}