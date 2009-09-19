using System;

namespace Rhino.Mocks
{
  // Static methods for working with RhinoMocks using AAA syntax
  public partial class MockRepository
  {
    /// <summary>Generates a stub without needing a <see cref="MockRepository"/></summary>
    /// <param name="argumentsForConstructor">Arguments for <typeparamref name="T"/>'s constructor</param>
    /// <typeparam name="T">The <see cref="Type"/> of stub to create.</typeparam>
    /// <returns>The stub</returns>
    /// <seealso cref="Stub{T}"/>
    public static T GenerateStub<T>(params object[] argumentsForConstructor) where T : class
    {
      return CreateMockInReplay(repo => (T)repo.Stub(typeof(T), argumentsForConstructor));
    }

    /// <summary>Generates a stub without needing a <see cref="MockRepository"/></summary>
    /// <param name="type">The <see cref="Type"/> of stub.</param>
    /// <param name="argumentsForConstructor">Arguments for the <paramref name="type"/>'s constructor.</param>
    /// <returns>The stub</returns>
    /// <seealso cref="Stub"/>
    public static object GenerateStub(Type type, params object[] argumentsForConstructor)
    {
      return CreateMockInReplay(repo => repo.Stub(type, argumentsForConstructor));
    }

    /// <summary>Generate a mock object without needing a <see cref="MockRepository"/></summary>
    /// <typeparam name="T">type <see cref="Type"/> of mock object to create.</typeparam>
    /// <param name="argumentsForConstructor">Arguments for <typeparamref name="T"/>'s constructor</param>
    /// <returns>the mock object</returns>
    /// <seealso cref="DynamicMock{T}"/>
    public static T GenerateMock<T>(params object[] argumentsForConstructor) where T : class
    {
      return CreateMockInReplay(r => r.DynamicMock<T>(argumentsForConstructor));
    }

    /// <summary>Generate a multi-mock object without needing a <see cref="MockRepository"/></summary>
    /// <typeparam name="T">The <c>typeof</c> object to generate a mock for.</typeparam>
    /// <typeparam name="TMultiMockInterface1">A second interface to generate a multi-mock for.</typeparam>
    /// <param name="argumentsForConstructor">Arguments for <typeparamref name="T"/>'s constructor</param>
    /// <returns>the multi-mock object</returns>
    /// <seealso cref="DynamicMultiMock(System.Type,System.Type[],object[])"/>
    public static T GenerateMock<T, TMultiMockInterface1>(params object[] argumentsForConstructor)
    {
      return (T) GenerateMock(typeof (T), new Type[] {typeof (TMultiMockInterface1)}, argumentsForConstructor);
    }

    /// <summary>Generate a multi-mock object without without needing a <see cref="MockRepository"/></summary>
    /// <typeparam name="T">The <c>typeof</c> object to generate a mock for.</typeparam>
    /// <typeparam name="TMultiMockInterface1">An interface to generate a multi-mock for.</typeparam>
    /// <typeparam name="TMultiMockInterface2">A second interface to generate a multi-mock for.</typeparam>
    /// <param name="argumentsForConstructor">Arguments for <typeparamref name="T"/>'s constructor</param>
    /// <returns>the multi-mock object</returns>
    /// <seealso cref="DynamicMultiMock(Type,Type[],object[])"/>
    public static T GenerateMock<T, TMultiMockInterface1, TMultiMockInterface2>(params object[] argumentsForConstructor)
    {
      return (T) GenerateMock(typeof (T), new Type[] {typeof (TMultiMockInterface1), typeof (TMultiMockInterface2)}, argumentsForConstructor);
    }

    /// <summary>Creates a multi-mock without without needing a <see cref="MockRepository"/></summary>
    /// <param name="type">The type of mock to create, this can be a class</param>
    /// <param name="extraTypes">Any extra interfaces to add to the multi-mock, these can only be interfaces.</param>
    /// <param name="argumentsForConstructor">Arguments for <paramref name="type"/>'s constructor</param>
    /// <returns>the multi-mock object</returns>
    /// <seealso cref="DynamicMultiMock(System.Type,System.Type[],object[])"/>
    public static object GenerateMock(Type type, Type[] extraTypes, params object[] argumentsForConstructor)
    {
      return CreateMockInReplay(r => r.DynamicMultiMock(type, extraTypes, argumentsForConstructor));
    }

    ///<summary>Creates a strict mock without without needing a <see cref="MockRepository"/></summary>
    ///<param name="argumentsForConstructor">Any arguments required for the <typeparamref name="T"/>'s constructor</param>
    ///<typeparam name="T">The type of mock object to create.</typeparam>
    ///<returns>The mock object with strict replay semantics</returns>
    /// <seealso cref="StrictMock{T}"/>
    public static T GenerateStrictMock<T>(params object[] argumentsForConstructor)
    {
      return CreateMockInReplay(r => r.StrictMock<T>(argumentsForConstructor));
    }

    ///<summary>Creates a strict multi-mock without needing a <see cref="MockRepository"/></summary>
    ///<param name="argumentsForConstructor">Any arguments required for the <typeparamref name="T"/>'s constructor</param>
    ///<typeparam name="T">The type of mock object to create, this can be a class.</typeparam>
    ///<typeparam name="TMultiMockInterface1">An interface to generate a multi-mock for, this must be an interface!</typeparam>
    ///<returns>The multi-mock object with strict replay semantics</returns>
    /// <seealso cref="StrictMultiMock(System.Type,System.Type[],object[])"/>
    public static T GenerateStrictMock<T, TMultiMockInterface1>(params object[] argumentsForConstructor)
    {
      return (T)GenerateStrictMock(typeof(T), new Type[] { typeof(TMultiMockInterface1) }, argumentsForConstructor);
    }

    ///<summary>Creates a strict multi-mock without needing a <see cref="MockRepository"/></summary>
    ///<param name="argumentsForConstructor">Any arguments required for the <typeparamref name="T"/>'s constructor</param>
    ///<typeparam name="T">The type of mock object to create, this can be a class.</typeparam>
    ///<typeparam name="TMultiMockInterface1">An interface to generate a multi-mock for, this must be an interface!</typeparam>
    ///<typeparam name="TMultiMockInterface2">A second interface to generate a multi-mock for, this must be an interface!</typeparam>
    ///<returns>The multi-mock object with strict replay semantics</returns>
    ///<seealso cref="StrictMultiMock(System.Type,System.Type[],object[])"/>
    public static T GenerateStrictMock<T, TMultiMockInterface1, TMultiMockInterface2>(params object[] argumentsForConstructor)
    {
      return (T)GenerateStrictMock(typeof(T), new Type[] { typeof(TMultiMockInterface1), typeof(TMultiMockInterface2) }, argumentsForConstructor);
    }

    ///<summary>Creates a strict multi-mock without needing a <see cref="MockRepository"/></summary>
    ///<param name="type">The type of mock object to create, this can be a class</param>
    ///<param name="extraTypes">Any extra interfaces to generate a multi-mock for, these must be interaces!</param>
    ///<param name="argumentsForConstructor">Any arguments for the <paramref name="type"/>'s constructor</param>
    ///<returns>The strict multi-mock object</returns>
    /// <seealso cref="StrictMultiMock(System.Type,System.Type[],object[])"/>
    public static object GenerateStrictMock(Type type, Type[] extraTypes, params object[] argumentsForConstructor)
    {
      if (extraTypes == null) extraTypes = new Type[0];
      if (argumentsForConstructor == null) argumentsForConstructor = new object[0];

      return CreateMockInReplay(r => r.StrictMultiMock(type, extraTypes, argumentsForConstructor));
    }

    ///<summary>
    ///</summary>
    ///<param name="argumentsForConstructor"></param>
    ///<typeparam name="T"></typeparam>
    ///<returns></returns>
    public static T GeneratePartialMock<T>(params object[] argumentsForConstructor)
    {
      return (T)GeneratePartialMock(typeof(T), new Type[0], argumentsForConstructor);
    }

    ///<summary>
    ///</summary>
    ///<param name="argumentsForConstructor"></param>
    ///<typeparam name="T"></typeparam>
    ///<typeparam name="TMultiMockInterface1"></typeparam>
    ///<returns></returns>
    public static T GeneratePartialMock<T, TMultiMockInterface1>(params object[] argumentsForConstructor)
    {
      return (T)GeneratePartialMock(typeof(T), new Type[] { typeof(TMultiMockInterface1) }, argumentsForConstructor);
    }

    ///<summary>
    ///</summary>
    ///<param name="argumentsForConstructor"></param>
    ///<typeparam name="T"></typeparam>
    ///<typeparam name="TMultiMockInterface1"></typeparam>
    ///<typeparam name="TMultiMockInterface2"></typeparam>
    ///<returns></returns>
    public static T GeneratePartialMock<T, TMultiMockInterface1, TMultiMockInterface2>(params object[] argumentsForConstructor)
    {
      return (T)GeneratePartialMock(typeof(T), new Type[] { typeof(TMultiMockInterface1), typeof(TMultiMockInterface2) }, argumentsForConstructor);
    }

    ///<summary>
    ///</summary>
    ///<param name="type"></param>
    ///<param name="extraTypes"></param>
    ///<param name="argumentsForConstructor"></param>
    ///<returns></returns>
    public static object GeneratePartialMock(Type type, Type[] extraTypes, params object[] argumentsForConstructor)
    {
      return CreateMockInReplay(r => r.PartialMultiMock(type, extraTypes, argumentsForConstructor));
    }

    /// <summary>
    /// Generate a mock object with dynamic replay semantics and remoting without needing the mock repository
    /// </summary>
    public static T GenerateDynamicMockWithRemoting<T>(params object[] argumentsForConstructor)
    {
      return CreateMockInReplay(r => r.DynamicMockWithRemoting<T>(argumentsForConstructor));
    }

    /// <summary>
    /// Generate a mock object with strict replay semantics and remoting without needing the mock repository
    /// </summary>
    public static T GenerateStrictMockWithRemoting<T>(params object[] argumentsForConstructor) where T : class
    {
      return CreateMockInReplay(r => r.StrictMockWithRemoting<T>(argumentsForConstructor));
    }

    /// <summary>Helper method to create a mock object without a repository instance and put the object back into replay mode.</summary>
    /// <typeparam name="T">The type of mock object to create</typeparam>
    /// <param name="createMock">A delegate that uses a mock repository instance to create the underlying mock</param>
    /// <returns>The mock object in the replay mode.</returns>
    private static T CreateMockInReplay<T>(Func<MockRepository, T> createMock)
    {
      var repository = new MockRepository();
      var mockObject = createMock(repository);
      repository.Replay(mockObject);
      return mockObject;
    }
  }
}