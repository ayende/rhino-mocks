namespace Rhino.Mocks.Impl.InvocationSpecifications
{
    ///<summary>
    ///</summary>
    public interface ISpecification<T>
    {
        ///<summary>
        ///</summary>
        bool IsSatisfiedBy(T item);
    }
}