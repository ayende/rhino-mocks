namespace Rhino.Mocks
{
    using System;
    /// <summary>
    /// This delegate is compatible with the System.Func{T,R} signature
    /// We have to define our own to get compatability with 2.0
    /// </summary>
    public delegate R Function<T, R>(T t);

#if FOR_NET_2_0
    ///<summary>
    /// This is here to allow compiling on the 2.0 platform with extension methods
    ///</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExtensionAttribute : Attribute
    {
    }
  
#endif
}