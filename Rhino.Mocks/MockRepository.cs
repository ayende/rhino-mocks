#region license

// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Generated;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Impl.Invocation;
using Rhino.Mocks.Impl.RemotingMock;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.MethodRecorders;

namespace Rhino.Mocks
{
    /*
     * class: MockRepository
     * The MockRepository is the main interaction point with Rhino Mocks.
     * 
     * Common usage pattern is to create the <Rhino.Mocks.MockRepository> on [SetUp]
     * and then create mock objects using either <Rhino.Mocks.MockRepository.CreateMock> or
     * <Rhino.Mocks.MockRepository.DynamicMock> and setup expectations on the mock object(s) by
     * callling their methods. A call to <Rhino.Mocks.MockRepository.ReplayAll> would move the mock
     * object(s) to replay state, a call to <Rhino.Mocks.MockRepository.VerifyAll> is made from the 
     * [TearDown] method.
     * 
     * Thread Safety:
     * MockRepository is capable of verifying in multiply threads, but recording in multiply threads
     * is not recommended. If you need to do so you _must_ use the <Rhino.Mocks.Expect.On> and 
     * <Rhino.Mocks.LastCall.On> methods and not <Rhino.Mocks.Expect.Call> and 
     * <LastCall>'s various methods. 
     * 
     * Code Sample:
     * 
     * (start code)
     *  MockRepository mocks; 
     * 
     *  [SetUp]
     *  public void Setup()
     *  {
     *      mocks = new MockRepository();
     *  }
     *
     *    [Test]
     *    public void CallMethodOnObject()
     *    {
     *        
     *        IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
     *        // Setting up an expectation for a call to IDemo.VoidNoArg
     *        demo.VoidNoArg();
     *        mocks.ReplayAll();
     *        // Fullifying the expectation for call to IDemo.VoidNoArg
     *        demo.VoidNoArg();
     *    }
     *
     *  [TearDown]
     *  public void Teardown()
     *  {
     *         mocks.VerifyAll();
     *  }
     * (end)
     * 
     * Class Responsbilities:
     * 
     *  - Create and manage mock object throughout their life time
     *     
     * See Also:
     * - <Rhino.Mocks.MockRepository.CreateMock>
     * - <Rhino.Mocks.MockRepository.DynamicMock>
     * - <Rhino.Mocks.MockRepository.ReplayAll>
     * - <Rhino.Mocks.MockRepository.VerifyAll>
    */

    /// <summary>
    /// Creates proxied instances of types.
    /// </summary>
    public partial class MockRepository
    {
        /// <summary>
        ///  Delegate: CreateMockState
        ///  This is used internally to cleanly handle the creation of different 
        ///  RecordMockStates.
        /// </summary>
        protected delegate IMockState CreateMockState(IMockedObject mockedObject);

        #region Variables

        /*
         * Variable: generatorMap
         * A static variable that is used to hold a map of Types to ProxyGenerators
         * 
         */

        /// <summary>
        /// This is a map of types to ProxyGenerators.
        /// </summary>
        private static readonly IDictionary<Type, ProxyGenerator> generatorMap = new Dictionary<Type, ProxyGenerator>();

        /*
         * Variable: lastRepository
         * A static variable that is used to hold the repository that last had a method call
         * on one of its mock objects.
         * 
         */

        /// <summary>
        /// This is used to record the last repository that has a method called on it.
        /// </summary>
        internal static MockRepository lastRepository;

        /*
         * Var: lastProxy
         * The last proxy that had a method call for _this_ repository
         */

        /// <summary>
        /// this is used to get to the last proxy on this repository.
        /// </summary>
        internal IMockedObject lastMockedObject;

        private static readonly DelegateTargetInterfaceCreator delegateTargetInterfaceCreator =
            new DelegateTargetInterfaceCreator();

        /// <summary>
        /// For mock delegates, maps the proxy instance from intercepted invocations
        /// back to the delegate that was originally returned to client code, if any.
        /// </summary>
        protected IDictionary delegateProxies;

        /// <summary>
        /// All the proxies in the mock repositories
        /// </summary>
        protected ProxyStateDictionary proxies;

        private readonly Stack recorders;
        private readonly IMethodRecorder rootRecorder;
        /// <summary>
        /// This is here because we can't put it in any of the recorders, since repeatable methods
        /// have no orderring, and if we try to handle them using the usual manner, we would get into
        /// wierd situations where repeatable method that was defined in an orderring block doesn't
        /// exists until we enter this block.
        /// </summary>
        private readonly ProxyMethodExpectationsDictionary repeatableMethods;

        private ProxyGenerationOptions proxyGenerationOptions;
        private InvocationVisitorsFactory invocationVisitorsFactory;

        #endregion

        #region Properties

        /*
         * Property: Recorder
         * Gets the current recorder for the repository.
         */

        /// <summary>
        /// Gets the recorder.
        /// </summary>
        /// <value></value>
        internal IMethodRecorder Recorder
        {
            get { return recorders.Peek() as IMethodRecorder; }
        }

        #endregion

        #region c'tors

        /* function: MockRepository
         * Create a new instance of MockRepository
         */

        /// <summary>
        /// Creates a new <see cref="MockRepository"/> instance.
        /// </summary>
        public MockRepository()
        {
            proxyGenerationOptions = new ProxyGenerationOptions
            {
                AttributesToAddToGeneratedTypes = 
                    {
                        new __ProtectAttribute()
                    }
            };
            recorders = new Stack();
            repeatableMethods = new ProxyMethodExpectationsDictionary();
            rootRecorder = new UnorderedMethodRecorder(repeatableMethods);
            recorders.Push(rootRecorder);
            proxies = new ProxyStateDictionary();
            delegateProxies = new Hashtable(MockedObjectsEquality.Instance);
            invocationVisitorsFactory = new InvocationVisitorsFactory();

            // clean up Arg data to avoid the static data to be carried from one unit test
            // to another.
            ArgManager.Clear();
        }


        #endregion

        #region Methods

        /*
         * Method: Ordered
         * Moves the _entire_ <MockRepository> to use ordered recording.
         * 
         * This call is only valid during the recording phase.
         * This call affects all mock objects that were created from this repository.
         * 
         * The orderring is ended when the returned IDisposable's Dispose() method is called.
         * (start code)
         *    [Test]
          *    public void CallMethodOnObject()
          *    {
          *        IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
         *        //Moving to ordered mocking.
          *        using(mocks.Ordered()
         *        {
         *            demo.VoidNoArg();
         *            demo.IntNoArg();
         *        }
         *        //Must exit the ordering before calling 
         *        mocks.ReplayAll();
         *        //If we would try to call them in any other order, the test would fail
         *        demo.VoidNoArg();
         *        demo.IntNoArg();
         *    }
         * (end)
         * 
         */

        /// <summary>
        /// Move the repository to ordered mode
        /// </summary>
        public IDisposable Ordered()
        {
            return new RecorderChanger(this, Recorder, new OrderedMethodRecorder(Recorder, repeatableMethods));
        }

        /*
         * Method: Unordered
         * Moves the _entire_ <MockRepository> to use unordered recording (the default).
         * 
         * This call is only valid during the recording phase.
         * This call affects all mock objects that were created from this repository.
         * 
         * (start code)
         *    [Test]
         *    public void CallMethodOnObject()
         *    {
         *        IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
         *        //Moving to ordered mocking.
         *        using(mocks.Ordered()
         *        {
         *            demo.VoidNoArg();
         *            using(mocks.Unordered()
         *            {
         *                demo.VoidNoArg();
         *                demo.IntNoArg();
         *            }
         *            demo.IntNoArg();
         *        }
         *        //Must exit the ordering before calling 
         *        mocks.ReplayAll();
         *        //The expectations we set up is:
         *        // 1. demo.VoidNoArgs();
         *        // 2. in any order:
         *        //        1. demo.VoidNoArg();
         *        //        2. demo.IntNoArg();
         *        // 3. demo.IntNoArg();
         *        demo.VoidNoArg();
         *        demo.IntNoArg();
         *        demo.VoidNoArg();
         *        demo.IntNoArg();
         *    }
         */

        /// <summary>
        /// Move the repository to un-ordered mode
        /// </summary>
        public IDisposable Unordered()
        {
            return new RecorderChanger(this, Recorder, new UnorderedMethodRecorder(Recorder, repeatableMethods));
        }

        /*
         * Method: CreateMock
         * Create a mock object with strict semantics.
         * Strict semantics means that any call that wasn't explicitly recorded is considered an
         * error and would cause an exception to be thrown. 
         */

        /// <summary>
        /// Creates a mock for the specified type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        [Obsolete("Use StrictMock instead")]
        public object CreateMock(Type type, params object[] argumentsForConstructor)
        {
            return StrictMock(type, argumentsForConstructor);
        }

        /// <summary>
        /// Creates a strict mock for the specified type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public object StrictMock(Type type, params object[] argumentsForConstructor)
        {
            if (ShouldUseRemotingProxy(type, argumentsForConstructor))
                return RemotingMock(type, CreateRecordState);
            return StrictMultiMock(type, new Type[0], argumentsForConstructor);
        }

        /// <summary>
        /// Creates a remoting mock for the specified type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        [Obsolete("Use StrictMockWithRemoting instead")]
        public object CreateMockWithRemoting(Type type, params object[] argumentsForConstructor)
        {
            return StrictMockWithRemoting(type, argumentsForConstructor);
        }

        /// <summary>
        /// Creates a strict remoting mock for the specified type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public object StrictMockWithRemoting(Type type, params object[] argumentsForConstructor)
        {
            return RemotingMock(type, CreateRecordState);
        }

        /// <summary>
        /// Creates a remoting mock for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        /// <returns></returns>
        [Obsolete("Use StrictMockWithRemoting instead")]
        public T CreateMockWithRemoting<T>(params object[] argumentsForConstructor)
        {
            return StrictMockWithRemoting<T>(argumentsForConstructor);
        }

        /// <summary>
        /// Creates a strict remoting mock for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        /// <returns></returns>
        public T StrictMockWithRemoting<T>(params object[] argumentsForConstructor)
        {
            return (T)RemotingMock(typeof(T), CreateRecordState);
        }

        /// <summary>
        /// Creates a mock from several types, with strict semantics.
        /// Only <paramref name="mainType"/> may be a class.
        /// </summary>
        [Obsolete("Use StrictMultiMock instead")]
        public object CreateMultiMock(Type mainType, params Type[] extraTypes)
        {
            return StrictMultiMock(mainType, extraTypes);
        }

        /// <summary>
        /// Creates a strict mock from several types, with strict semantics.
        /// Only <paramref name="mainType"/> may be a class.
        /// </summary>
        public object StrictMultiMock(Type mainType, params Type[] extraTypes)
        {
            return StrictMultiMock(mainType, extraTypes, new object[0]);
        }

        /// <summary>
        /// Creates a mock from several types, with strict semantics.
        /// Only <paramref name="mainType"/> may be a class.
        /// </summary>
        /// <param name="mainType">The main type to mock.</param>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class.</param>
        [Obsolete("Use StrictMultiMock instead")]
        public object CreateMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor)
        {
            return StrictMultiMock(mainType, extraTypes, argumentsForConstructor);
        }

        /// <summary>
        /// Creates a strict mock from several types, with strict semantics.
        /// Only <paramref name="mainType"/> may be a class.
        /// </summary>
        /// <param name="mainType">The main type to mock.</param>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class.</param>
        public object StrictMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor)
        {
            if (argumentsForConstructor == null) argumentsForConstructor = new object[0];
            return CreateMockObject(mainType, CreateRecordState, extraTypes, argumentsForConstructor);
        }

        /// <summary>
        /// Creates a mock from several types, with dynamic semantics.
        /// Only <paramref name="mainType"/> may be a class.
        /// </summary>
        /// <param name="mainType">The main type to mock.</param>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        public object DynamicMultiMock(Type mainType, params Type[] extraTypes)
        {
            return DynamicMultiMock(mainType, extraTypes, new object[0]);
        }

        /// <summary>
        /// Creates a mock from several types, with dynamic semantics.
        /// Only <paramref name="mainType"/> may be a class.
        /// </summary>
        /// <param name="mainType">The main type to mock.</param>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class.</param>
        public object DynamicMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor)
        {
            return CreateMockObject(mainType, CreateDynamicRecordState, extraTypes, argumentsForConstructor);
        }

        /*
         * Method: DynamicMock
         * Create a mock object with dynamic semantics.
         * Dynamic semantics means that any call that wasn't explicitly recorded is accepted and a
         * null or zero is returned (if there is a return value).
         */

        /// <summary>Creates a dynamic mock for the specified type.</summary>
        /// <param name="type">Type.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public object DynamicMock(Type type, params object[] argumentsForConstructor)
        {
            if (ShouldUseRemotingProxy(type, argumentsForConstructor))
                return RemotingMock(type, CreateDynamicRecordState);
            return DynamicMultiMock(type, new Type[0], argumentsForConstructor);
        }

        /// <summary>Creates a dynamic mock for the specified type.</summary>
        /// <param name="type">Type.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public object DynamicMockWithRemoting(Type type, params object[] argumentsForConstructor)
        {
            return RemotingMock(type, CreateDynamicRecordState);
        }

        /// <summary>Creates a dynamic mock for the specified type.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        /// <returns></returns>
        public T DynamicMockWithRemoting<T>(params object[] argumentsForConstructor)
        {
            return (T)RemotingMock(typeof(T), CreateDynamicRecordState);
        }

        /// <summary>Creates a mock object that defaults to calling the class methods if no expectation is set on the method.</summary>
        /// <param name="type">Type.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor.</param>
        public object PartialMock(Type type, params object[] argumentsForConstructor)
        {
            return PartialMultiMock(type, new Type[0], argumentsForConstructor);
        }

        /// <summary>Creates a mock object that defaults to calling the class methods.</summary>
        /// <param name="type">Type.</param>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        public object PartialMultiMock(Type type, params Type[] extraTypes)
        {
            return PartialMultiMock(type, extraTypes, new object[0]);
        }

        /// <summary>Creates a mock object that defaults to calling the class methods.</summary>
        /// <param name="type">Type.</param>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor.</param>
        public object PartialMultiMock(Type type, Type[] extraTypes, params object[] argumentsForConstructor)
        {
            if (type.IsInterface)
                throw new InvalidOperationException("Can't create a partial mock from an interface");
            List<Type> extraTypesWithMarker = new List<Type>(extraTypes);
            extraTypesWithMarker.Add(typeof(IPartialMockMarker));
            return CreateMockObject(type, CreatePartialRecordState, extraTypesWithMarker.ToArray(), argumentsForConstructor);
        }

        /// <summary>Creates a mock object using remoting proxies</summary>
        /// <param name="type">Type to mock - must be MarshalByRefObject</param>
        /// <returns>Mock object</returns>
        /// <remarks>Proxy mock can mock non-virtual methods, but not static methods</remarks>
        /// <param name="factory">Creates the mock state for this proxy</param>
        private object RemotingMock(Type type, CreateMockState factory)
        {
            ProxyInstance rhinoProxy = new ProxyInstance(this, type);
            RhinoInterceptor interceptor = new RhinoInterceptor(this, rhinoProxy,invocationVisitorsFactory.CreateStandardInvocationVisitors(rhinoProxy, this));
            object transparentProxy = new RemotingMockGenerator().CreateRemotingMock(type, interceptor, rhinoProxy);
            IMockState value = factory(rhinoProxy);
            proxies.Add(transparentProxy, value);
            return transparentProxy;
        }

        /// <summary>
        /// Cause the mock state to change to replay, any further call is compared to the 
        /// ones that were called in the record state.
        /// </summary>
        /// <remarks>This method *cannot* be called from inside an ordering.</remarks>
        /// <param name="obj">the object to move to replay state</param>
        public void Replay(object obj)
        {
            ReplayCore(obj, true);
        }

        /// <summary>
        /// Cause the mock state to change to replay, any further call is compared to the 
        /// ones that were called in the record state.
        /// </summary>
        /// <param name="obj">the object to move to replay state</param>
        /// <param name="checkInsideOrdering"></param>
        protected internal void ReplayCore(object obj, bool checkInsideOrdering)
        {
            if (checkInsideOrdering)
                NotInsideOrderring();

            IsMockObjectFromThisRepository(obj);
            ClearLastProxy(obj);
            IMockState state = proxies[obj];
            proxies[obj] = state.Replay();
            foreach (IMockedObject dependentMock in GetMockedObject(obj).DependentMocks)
            {
                ReplayCore(dependentMock, checkInsideOrdering);
            }
        }

        /// <summary>Move the mocked object back to record state.<para>You can (and it's recommended) to run {Verify()} before you use this method.</para></summary>
        /// <remarks>Will delete all current expectations!</remarks>
        public void BackToRecord(object obj)
        {
            BackToRecord(obj, BackToRecordOptions.All);
        }

        /// <summary>
        /// Move the mocked object back to record state.
        /// Optionally, can delete all current expectations, but allows more granularity about how
        /// it would behave with regard to the object state.
        /// </summary>
        public void BackToRecord(object obj, BackToRecordOptions options)
        {
            IsMockObjectFromThisRepository(obj);

            if ((options & BackToRecordOptions.Expectations) == BackToRecordOptions.Expectations)
            {
                foreach (IExpectation expectation in rootRecorder.GetAllExpectationsForProxy(obj))
                {
                    rootRecorder.RemoveExpectation(expectation);
                }
                rootRecorder.RemoveAllRepeatableExpectationsForProxy(obj);
            }

            GetMockedObject(obj).ClearState(options);

            proxies[obj] = proxies[obj].BackToRecord();
            foreach (IMockedObject dependentMock in GetMockedObject(obj).DependentMocks)
            {
                BackToRecord(dependentMock, options);
            }
        }

        /*
         * Method: Verify
         * Verifies that all expectations has been met for a single mock object.
         * After calling this method and action taken on the mock object would result in an 
         * exception even if the object is a dynamic mock.
         */

        /// <summary>
        /// Verify that all the expectations for this object were fulfilled.
        /// </summary>
        /// <param name="obj">the object to verify the expectations for</param>
        public void Verify(object obj)
        {
            IsMockObjectFromThisRepository(obj);
            try
            {
                proxies[obj].Verify();
                foreach (IMockedObject dependentMock in GetMockedObject(obj).DependentMocks)
                {
                    Verify(dependentMock);
                }
            }
            finally
            {
                //This is needed because there might be an exception in verifying
                //and I still need the mock state to move to verified.
                proxies[obj] = proxies[obj].VerifyState;
            }
        }

        /*
         * Method: LastMethodCall
         * Gets the method options for the last call on mockedInstance
         */

        /// <summary>
        /// Get the method options for the last call on
        /// mockedInstance.
        /// </summary>
        /// <param name="mockedInstance">The mock object</param>
        /// <returns>Method options for the last call</returns>
        internal IMethodOptions<T> LastMethodCall<T>(object mockedInstance)
        {
            object mock = GetMockObjectFromInvocationProxy(mockedInstance);
            IsMockObjectFromThisRepository(mock);
            return proxies[mock].GetLastMethodOptions<T>();
        }

        #endregion

        #region Implementation Details

        /*
         * Method: MethodCall
         * Handles a method call for a mock object.
         */

        internal object MethodCall(IInvocation invocation, object proxy, MethodInfo method, object[] args)
        {
            //This can happen only if a vritual method call originated from 
            //the constructor, before Rhino Mocks knows about the existance 
            //of this proxy. Those type of calls will be ignored and not count
            //as expectations, since there is not way to relate them to the 
            //proper state.
            if (proxies.ContainsKey(proxy) == false)
            {
                //We allow calls to virtual methods from the ctor only for partial mocks.
                if (proxy is IPartialMockMarker)
                {
                    invocation.Proceed();
                    return invocation.ReturnValue;
                }
                return null;
            }
            IMockState state = proxies[proxy];
            GetMockedObject(proxy).MethodCall(method, args);
            return state.MethodCall(invocation, method, args);
        }

        /// <summary>
        /// Maps an invocation proxy back to the mock object instance that was originally
        /// returned to client code which might have been a delegate to this proxy.
        /// </summary>
        /// <param name="invocationProxy">The mock object proxy from the intercepted invocation</param>
        /// <returns>The mock object</returns>
        internal object GetMockObjectFromInvocationProxy(object invocationProxy)
        {
            object proxy = delegateProxies[invocationProxy];
            if (proxy != null) return proxy;
            return invocationProxy;
        }

        private IMockState CreateRecordState(IMockedObject mockedObject)
        {
            return new RecordMockState(mockedObject, this);
        }

        private IMockState CreateDynamicRecordState(IMockedObject mockedObject)
        {
            return new RecordDynamicMockState(mockedObject, this);
        }

        private IMockState CreatePartialRecordState(IMockedObject mockedObject)
        {
            return new RecordPartialMockState(mockedObject, this);
        }

        private void NotInsideOrderring()
        {
            if (Recorder != rootRecorder)
                throw new InvalidOperationException(
                    "Can't start replaying because Ordered or Unordered properties were call and not yet disposed.");
        }

        private void ClearLastProxy(object obj)
        {
            if (GetMockedObjectOrNull(obj) == lastMockedObject)
                lastMockedObject = null;
        }

        private object MockClass(CreateMockState mockStateFactory, Type type, Type[] extras, object[] argumentsForConstructor)
        {
            if (type.IsSealed)
                throw new NotSupportedException("Can't create mocks of sealed classes");
            List<Type> implementedTypesForGenericInvocationDiscoverability = new List<Type>(extras);
            implementedTypesForGenericInvocationDiscoverability.Add(type);
            ProxyInstance proxyInstance = new ProxyInstance(this, implementedTypesForGenericInvocationDiscoverability.ToArray());
            RhinoInterceptor interceptor = new RhinoInterceptor(this, proxyInstance,invocationVisitorsFactory.CreateStandardInvocationVisitors(proxyInstance, this));
            ArrayList types = new ArrayList();
            types.AddRange(extras);
            types.Add(typeof(IMockedObject));
            object proxy;
            try
            {
                proxyGenerationOptions = ProxyGenerationOptions.Default;
                proxy = GetProxyGenerator(type).CreateClassProxy(type, (Type[])types.ToArray(typeof(Type)),
                                                   proxyGenerationOptions,
                                                   argumentsForConstructor, interceptor);
            }
            catch (MissingMethodException mme)
            {
                throw new MissingMethodException("Can't find a constructor with matching arguments", mme);
            }
            catch (TargetInvocationException tie)
            {
                throw new Exception("Exception in constructor: " + tie.InnerException, tie.InnerException);
            }
            IMockedObject mockedObject = (IMockedObject)proxy;
            mockedObject.ConstructorArguments = argumentsForConstructor;
            IMockState value = mockStateFactory(mockedObject);
            proxies.Add(proxy, value);
            GC.SuppressFinalize(proxy);//avoid issues with expectations created/validated on the finalizer thread
            return proxy;
        }

        private object MockInterface(CreateMockState mockStateFactory, Type type, Type[] extras)
        {
            object proxy;
            List<Type> implementedTypesForGenericInvocationDiscoverability = new List<Type>(extras);
            implementedTypesForGenericInvocationDiscoverability.Add(type);
            ProxyInstance proxyInstance = new ProxyInstance(this,
                                                             implementedTypesForGenericInvocationDiscoverability
                                                                 .ToArray());
            RhinoInterceptor interceptor = new RhinoInterceptor(this, proxyInstance,invocationVisitorsFactory.CreateStandardInvocationVisitors(proxyInstance, this));

            List<Type> types = new List<Type>();
            types.AddRange(extras);
            types.Add(typeof(IMockedObject));
            proxy =
                GetProxyGenerator(type).CreateInterfaceProxyWithoutTarget(type, types.ToArray(), proxyGenerationOptions, interceptor);
            IMockState value = mockStateFactory((IMockedObject)proxy);
            proxies.Add(proxy, value);
            return proxy;
        }

        private object MockDelegate(CreateMockState mockStateFactory, Type type)
        {
            if (typeof(Delegate).Equals(type))
                throw new InvalidOperationException("Cannot mock the Delegate base type.");

            object proxy;

            ProxyInstance proxyInstance = new ProxyInstance(this, type);
            RhinoInterceptor interceptor = new RhinoInterceptor(this, proxyInstance,invocationVisitorsFactory.CreateStandardInvocationVisitors(proxyInstance, this));

            Type[] types = new Type[] { typeof(IMockedObject) };
            var delegateTargetInterface = delegateTargetInterfaceCreator.GetDelegateTargetInterface(type);
            object target = GetProxyGenerator(type).CreateInterfaceProxyWithoutTarget(
                delegateTargetInterface,
                types, proxyGenerationOptions, interceptor);

            proxy = Delegate.CreateDelegate(type, target, delegateTargetInterface.Name+ ".Invoke");
            delegateProxies.Add(target, proxy);

            IMockState value = mockStateFactory(GetMockedObject(proxy));
            proxies.Add(proxy, value);
            return proxy;
        }

        /// <summary>This is provided to allow advance extention functionality, where Rhino Mocks standard functionality is not enough.</summary>
        /// <param name="type">The type to mock</param>
        /// <param name="factory">Delegate that create the first state of the mocked object (usualy the record state).</param>
        /// <param name="extras">Additional types to be implemented, this can be only interfaces </param>
        /// <param name="argumentsForConstructor">optional arguments for the constructor</param>
        /// <returns></returns>
        protected object CreateMockObject(Type type, CreateMockState factory, Type[] extras, params object[] argumentsForConstructor)
        {
            foreach (Type extraType in extras)
            {
                if (!extraType.IsInterface)
                {
                    throw new ArgumentException("Extra types must all be interfaces", "extras");
                }
            }

            if (type.IsInterface)
            {
                if (argumentsForConstructor != null && argumentsForConstructor.Length > 0)
                {
                    throw new ArgumentException(
                        "Constructor arguments should not be supplied when mocking an interface",
                        "argumentsForConstructor");
                }
                return MockInterface(factory, type, extras);
            }
            else if (typeof(Delegate).IsAssignableFrom(type))
            {
                if (argumentsForConstructor != null && argumentsForConstructor.Length > 0)
                {
                    throw new ArgumentException("Constructor arguments should not be supplied when mocking a delegate",
                                                "argumentsForConstructor");
                }
                return MockDelegate(factory, type);
            }
            else
                return MockClass(factory, type, extras, argumentsForConstructor);
        }

        private void IsMockObjectFromThisRepository(object obj)
        {
            if (proxies.ContainsKey(obj) == false)
                throw new ObjectNotMockFromThisRepositoryException(
                    "The object is not a mock object that belong to this repository.");
        }

        /// <summary>
        ///  Method: GetMockedObject
        ///  Get an IProxy from a mocked object instance, or throws if the 
        ///  object is not a mock object.
        /// </summary>
        protected internal static IMockedObject GetMockedObject(object mockedInstance)
        {
            IMockedObject mockedObj = GetMockedObjectOrNull(mockedInstance);
            if (mockedObj == null)
                throw new InvalidOperationException("The object '" + mockedInstance +
                                                    "' is not a mocked object.");
            return mockedObj;
        }

        /// <summary>
        /// Method: GetMockedObjectOrNull
        /// Get an IProxy from a mocked object instance, or null if the
        /// object is not a mock object.
        /// </summary>
        protected internal static IMockedObject GetMockedObjectOrNull(object mockedInstance)
        {
            Delegate mockedDelegate = mockedInstance as Delegate;

            if (mockedDelegate != null)
            {
                mockedInstance = mockedDelegate.Target;
            }

            // must be careful not to call any methods on mocked objects,
            // or it may cause infinite recursion
            if (mockedInstance is IMockedObject)
            {
                return (IMockedObject)mockedInstance;
            }

            if (RemotingMockGenerator.IsRemotingProxy(mockedInstance))
            {
                return RemotingMockGenerator.GetMockedObjectFromProxy(mockedInstance);
            }

            return null;
        }

        /// <summary>Pops the recorder.</summary>
        internal void PopRecorder()
        {
            if (recorders.Count > 1)
                recorders.Pop();
        }

        /// <summary>Pushes the recorder.</summary>
        /// <param name="newRecorder">New recorder.</param>
        internal void PushRecorder(IMethodRecorder newRecorder)
        {
            recorders.Push(newRecorder);
        }

        #endregion

        #region Convenience Methods

        /// <summary>
        /// All the mock objects in this repository will be moved
        /// to record state.
        /// </summary>
        public void BackToRecordAll()
        {
            BackToRecordAll(BackToRecordOptions.All);
        }

        /// <summary>
        /// All the mock objects in this repository will be moved
        /// to record state.
        /// </summary>
        public void BackToRecordAll(BackToRecordOptions options)
        {
            if (proxies.Count == 0)
                return;
            foreach (object key in new ArrayList(proxies.Keys))
            {
                BackToRecord(key, options);
            }
        }

        /*
         * Method: ReplayAll
         * Moves all the mock objects in the repository to replay state.
         * 
         * Note:
         * This method will skip any mock object that you've manually moved to replay state
         * by calling <Replay>
         */

        /// <summary>
        /// Replay all the mocks from this repository
        /// </summary>
        public void ReplayAll()
        {
            if (proxies.Count == 0)
                return;
            foreach (object key in new ArrayList(proxies.Keys))
            {
                if (proxies[key] is RecordMockState)
                    Replay(key);
            }
        }

        /*
         * Method: VerifyAll
         * Verifies that all the expectations on all mock objects in the repository are met.
         * 
         * Note:
         * This method skip any mock objects that you've manually verified using <Verify>
         * 
         * Exception safety:
         * If an unexpected exception has been thrown (which would fail the test) and the Repository
         * still have unsatisfied expectations, this method will cause _another_ exception, that may
         * mask the real cause.
         * If this happens to you, you may need to avoid the using statement until you figure out what is wrong:
         * The using statement:
         * (start code)
         *    using(MockRepository mocks = new MockRepository())
         *    {
         *        // Some action that cause an unexpected exception
         *        // which would cause unsatisfied expectation and cause
         *        // VerifyAll() to fail.
         *    }
         * (end)
         *
         * The unrolled using statement:
         * (start code)
         *    MockRepository mocks = new MockRepository())
         *    //The afore mentioned action
         *    mocks.VerifyAll()//won't occur if an exception is thrown
         * (end)
         * 
         * This way you can get the real exception from the unit testing framework.
         */

        /// <summary>
        /// Verify all the mocks from this repository
        /// </summary>
        public void VerifyAll()
        {
            if (lastRepository == this)
                lastRepository = null;
            if (proxies.Keys.Count == 0)
                return;
            StringCollection validationErrors = new StringCollection();
            foreach (object key in new ArrayList(proxies.Keys))
            {
                if (proxies[key] is VerifiedMockState)
                    continue;
                try
                {
                    Verify(key);
                }
                catch (ExpectationViolationException e)
                {
                    validationErrors.Add(e.Message);
                }
            }
            if (validationErrors.Count == 0)
                return;
            if (validationErrors.Count == 1)
                throw new ExpectationViolationException(validationErrors[0]);
            StringBuilder sb = new StringBuilder();
            foreach (string validationError in validationErrors)
            {
                sb.AppendLine(validationError);
            }
            throw new ExpectationViolationException(sb.ToString());
        }

        /// <summary>
        /// Gets the replayer for this repository.
        /// </summary>
        /// <value></value>
        internal IMethodRecorder Replayer
        {
            get { return rootRecorder; }
        }

        /// <summary>
        /// Gets the last proxy which had a method call.
        /// </summary>
        internal static IMockedObject LastMockedObject
        {
            get
            {
                if (lastRepository == null)
                    return null;
                return lastRepository.lastMockedObject;
            }
        }

        /// <summary>
        /// Gets the proxy generator for a specific type. Having a single ProxyGenerator
        /// with multiple types linearly degrades the performance so this implementation
        /// keeps one ProxyGenerator per type. 
        /// </summary>
        protected virtual ProxyGenerator GetProxyGenerator(Type type)
        {
            lock (generatorMap)
            {
                if (!generatorMap.ContainsKey(type))
                {
                    generatorMap[type] = new ProxyGenerator();
                }

                return generatorMap[type];
            }
        }



        /// <summary>Set the exception to be thrown when verified is called.</summary>
        protected internal static void SetExceptionToBeThrownOnVerify(object proxy, ExpectationViolationException expectationViolationException)
        {
            MockRepository repository = GetMockedObject(proxy).Repository;
            if (repository.proxies.ContainsKey(proxy) == false)
                return;
            repository.proxies[proxy].SetExceptionToThrowOnVerify(expectationViolationException);
        }

        #endregion

        /// <summary>
        /// Creates a mock for the spesified type with strict mocking semantics.
        /// <para>Strict semantics means that any call that wasn't explicitly recorded is considered an error and would cause an exception to be thrown.</para>
        /// </summary>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        [Obsolete("Use StrictMock instead")]
        public T CreateMock<T>(params object[] argumentsForConstructor)
        {
            return StrictMock<T>(argumentsForConstructor);
        }

        /// <summary>
        /// Creates a mock for the spesified type with strict mocking semantics.
        /// <para>Strict semantics means that any call that wasn't explicitly recorded is considered an error and would cause an exception to be thrown.</para>
        /// </summary>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public T StrictMock<T>(params object[] argumentsForConstructor)
        {
            if (ShouldUseRemotingProxy(typeof(T), argumentsForConstructor))
                return (T)RemotingMock(typeof(T), CreateRecordState);
            return (T)CreateMockObject(typeof(T), CreateRecordState, new Type[0], argumentsForConstructor);
        }

        private static bool ShouldUseRemotingProxy(Type type, object[] argumentsForConstructor)
        {
            return typeof(MarshalByRefObject).IsAssignableFrom(type) &&
                (argumentsForConstructor == null || argumentsForConstructor.Length == 0);
        }

        /*
         * Method: DynamicMock<T>
         * Create a mock object of type T with dynamic semantics.
         * Dynamic semantics means that any call that wasn't explicitly recorded is accepted and a
         * null or zero is returned (if there is a return value).
         */

        /// <summary>
        /// Creates a dynamic mock for the specified type.
        /// </summary>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public T DynamicMock<T>(params object[] argumentsForConstructor)
            where T : class
        {
            if (ShouldUseRemotingProxy(typeof(T), argumentsForConstructor))
                return (T)RemotingMock(typeof(T), CreateDynamicRecordState);
            return (T)CreateMockObject(typeof(T), CreateDynamicRecordState, new Type[0], argumentsForConstructor);
        }

        /// <summary>
        /// Creates a mock object from several types.
        /// </summary>
        [Obsolete("Use StrictMultiMock instead")]
        public T CreateMultiMock<T>(params Type[] extraTypes)
        {
            return StrictMultiMock<T>(extraTypes);
        }

        /// <summary>
        /// Creates a strict mock object from several types.
        /// </summary>
        public T StrictMultiMock<T>(params Type[] extraTypes)
        {
            return (T)StrictMultiMock(typeof(T), extraTypes);
        }

        /// <summary>
        /// Create a mock object from several types with dynamic semantics.
        /// </summary>
        public T DynamicMultiMock<T>(params Type[] extraTypes)
        {
            return (T)DynamicMultiMock(typeof(T), extraTypes);
        }

        /// <summary>
        /// Create a mock object from several types with partial semantics.
        /// </summary>
        public T PartialMultiMock<T>(params Type[] extraTypes)
        {
            return (T)PartialMultiMock(typeof(T), extraTypes);
        }

        /// <summary>
        /// Create a mock object from several types with strict semantics.
        /// </summary>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        [Obsolete("Use StrictMultiMock instead")]
        public T CreateMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor)
        {
            return StrictMultiMock<T>(extraTypes, argumentsForConstructor);
        }


        /// <summary>
        /// Create a strict mock object from several types with strict semantics.
        /// </summary>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public T StrictMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor)
        {
            return (T)StrictMultiMock(typeof(T), extraTypes, argumentsForConstructor);
        }

        /// <summary>
        /// Create a mock object from several types with dynamic semantics.
        /// </summary>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public T DynamicMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor)
        {
            return (T)DynamicMultiMock(typeof(T), extraTypes, argumentsForConstructor);
        }

        /// <summary>
        /// Create a mock object from several types with partial semantics.
        /// </summary>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public T PartialMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor)
        {
            return (T)PartialMultiMock(typeof(T), extraTypes, argumentsForConstructor);
        }

        /*
         * Method: PartialMock
         * Create a mock object with from a class that defaults to calling the class methods
         * if no expectation is set on the method.
         * 
         */

        /// <summary>
        /// Create a mock object with from a class that defaults to calling the class methods
        /// </summary>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public T PartialMock<T>(params object[] argumentsForConstructor) where T : class
        {
            return (T)PartialMock(typeof(T), argumentsForConstructor);
        }

        /// <summary>
        /// Create a stub object, one that has properties and events ready for use, and 
        /// can have methods called on it. It requires an explicit step in order to create 
        /// an expectation for a stub.
        /// </summary>
        /// <param name="argumentsForConstructor">The arguments for constructor.</param>
        public T Stub<T>(params object[] argumentsForConstructor)
        {
            return (T)Stub(typeof(T), argumentsForConstructor);
        }

        /// <summary>
        /// Create a stub object, one that has properties and events ready for use, and
        /// can have methods called on it. It requires an explicit step in order to create
        /// an expectation for a stub.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="argumentsForConstructor">The arguments for constructor.</param>
        /// <returns>The stub</returns>
        public object Stub(Type type, params object[] argumentsForConstructor)
        {
            CreateMockState createStub = mockedObject => new StubRecordMockState(mockedObject, this);
            if (ShouldUseRemotingProxy(type, argumentsForConstructor))
                return RemotingMock(type, createStub);
            return CreateMockObject(type, createStub, new Type[0], argumentsForConstructor);
        }

        /// <summary>
        /// Returns true if the passed mock is currently in replay mode.
        /// </summary>
        /// <param name="mock">The mock to test.</param>
        /// <returns>True if the mock is in replay mode, false otherwise.</returns>
        public bool IsInReplayMode(object mock)
        {
            if (mock == null)
                throw new ArgumentNullException("mock");

            if (proxies.ContainsKey(mock))
            {
                return proxies[mock] is ReplayMockState;
            }

            throw new ArgumentException(mock + " is not a mock.", "mock");
        }

        /// <summary>
        /// Determines whether the specified proxy is a stub.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        protected internal static bool IsStub(object proxy)
        {
            MockRepository repository = GetMockedObject(proxy).Repository;
            IMockState mockState = repository.proxies[proxy];
            return mockState is StubRecordMockState || mockState is StubReplayMockState;
        }

        /// <summary>
        /// Register a call on a prperty behavior 
        /// </summary>
        /// <param name="instance"></param>
        protected internal void RegisterPropertyBehaviorOn(IMockedObject instance)
        {
            lastRepository = this;
            lastMockedObject = instance;
            proxies[instance].NotifyCallOnPropertyBehavior();
        }
    }
}
