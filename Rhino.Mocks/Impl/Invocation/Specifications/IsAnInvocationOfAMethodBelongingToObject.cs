using System;
using System.Reflection;
using Castle.DynamicProxy;
using Rhino.Mocks.Impl.InvocationSpecifications;

namespace Rhino.Mocks.Impl.Invocation.Specifications
{
    ///<summary>
    ///</summary>
    public class IsAnInvocationOfAMethodBelongingToObject : ISpecification<IInvocation>{
        private static MethodInfo[] objectMethods =
            new MethodInfo[]
            {
                typeof (object).GetMethod("ToString"), typeof (object).GetMethod("Equals", new Type[] {typeof (object)}),
                typeof (object).GetMethod("GetHashCode"), typeof (object).GetMethod("GetType")
            };

        ///<summary>
        ///</summary>
        public bool IsSatisfiedBy(IInvocation item)
        {
            return Array.IndexOf(objectMethods, item.Method) != -1;
        }
    }
}