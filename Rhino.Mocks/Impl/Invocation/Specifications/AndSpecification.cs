using Rhino.Mocks.Impl.InvocationSpecifications;

namespace Rhino.Mocks.Impl.Invocation.Specifications
{
    ///<summary>
    ///Summary for AndSpecification
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public class AndSpecification<T> : ISpecification<T> {
        ISpecification<T> left_side;
        ISpecification<T> right_side;

        ///<summary>
        ///</summary>
        public AndSpecification(ISpecification<T> left_side, ISpecification<T> right_side)
        {
            this.left_side = left_side;
            this.right_side = right_side;
        }

        ///<summary>
        ///</summary>
        public bool IsSatisfiedBy(T item)
        {
            return left_side.IsSatisfiedBy(item) && right_side.IsSatisfiedBy(item);
        }
    }
}