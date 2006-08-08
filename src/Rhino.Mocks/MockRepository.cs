using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Builder.CodeGenerators;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.MethodRecorders;
using Rhino.Mocks.Generated;
using System;


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
     *  	mocks = new MockRepository();
     *  }
     *
     *	[Test]
     *	public void CallMethodOnObject()
     *	{
     *		
     *		IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
     *		// Setting up an expectation for a call to IDemo.VoidNoArg
     *		demo.VoidNoArg();
     *		mocks.ReplayAll();
     *		// Fullifying the expectation for call to IDemo.VoidNoArg
     *		demo.VoidNoArg();
     *	}
     *
     *  [TearDown]
     *  public void Teardown()
     *  {
     *	 	mocks.VerifyAll();
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
    public
#if dotNet2
 partial
#endif
 class MockRepository
    {
        /* 
         * Delegate: CreateMockState
         * This is used internally to cleanly handle the creation of different 
         * RecordMockStates.
        */
        private delegate RecordMockState CreateMockState(IMockedObject mockedObject);

        #region Variables

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

        private static DelegateTargetInterfaceCreator delegateTargetInterfaceCreator = new DelegateTargetInterfaceCreator();

        /// <summary>
        /// For mock delegates, maps the proxy instance from intercepted invocations
        /// back to the delegate that was originally returned to client code, if any.
        /// </summary>
        private IDictionary delegateProxies;

        private static ProxyGenerator generator = new ProxyGenerator();
        private ProxyStateDictionary proxies;
        private Stack recorders;
        private IMethodRecorder rootRecorder;

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
            recorders = new Stack();
            rootRecorder = new UnorderedMethodRecorder();
            recorders.Push(rootRecorder);
            proxies = new ProxyStateDictionary();
#if dotNet2
            delegateProxies = new Hashtable(MockedObjectsEquality.Instance);
#else
            delegateProxies = new Hashtable(MockedObjectsEquality.Instance, MockedObjectsEquality.Instance);
#endif
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
		 *	[Test]
 		 *	public void CallMethodOnObject()
 		 *	{
 		 *		IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
		 *		//Moving to ordered mocking.
 		 *		using(mocks.Ordered()
		 *		{
		 *			demo.VoidNoArg();
		 *			demo.IntNoArg();
		 *		}
		 *		//Must exit the ordering before calling 
		 *		mocks.ReplayAll();
		 *		//If we would try to call them in any other order, the test would fail
		 *		demo.VoidNoArg();
		 *		demo.IntNoArg();
		 *	}
		 * (end)
		 * 
		 */
        /// <summary>
        /// Move the repository to ordered mode
        /// </summary>
        public IDisposable Ordered()
        {
            return new RecorderChanger(this, Recorder, new OrderedMethodRecorder(Recorder));
        }

        /*
         * Method: Unordered
         * Moves the _entire_ <MockRepository> to use unordered recording (the default).
         * 
         * This call is only valid during the recording phase.
         * This call affects all mock objects that were created from this repository.
         * 
         * (start code)
         *	[Test]
         *	public void CallMethodOnObject()
         *	{
         *		IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
         *		//Moving to ordered mocking.
         *		using(mocks.Ordered()
         *		{
         *			demo.VoidNoArg();
         *			using(mocks.Unordered()
         *			{
         *				demo.VoidNoArg();
         *				demo.IntNoArg();
         *			}
         *			demo.IntNoArg();
         *		}
         *		//Must exit the ordering before calling 
         *		mocks.ReplayAll();
         *		//The expectations we set up is:
         *		// 1. demo.VoidNoArgs();
         *		// 2. in any order:
         *		//		1. demo.VoidNoArg();
         *		//		2. demo.IntNoArg();
         *		// 3. demo.IntNoArg();
         *		demo.VoidNoArg();
         *		demo.IntNoArg();
         *		demo.VoidNoArg();
         *		demo.IntNoArg();
         *	}
         */
        /// <summary>
        /// Move the repository to un-ordered mode
        /// </summary>
        public IDisposable Unordered()
        {
            return new RecorderChanger(this, Recorder, new UnorderedMethodRecorder(Recorder));
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
        public object CreateMock(Type type, params object[] argumentsForConstructor)
        {
            return CreateMultiMock(type, new Type[0], argumentsForConstructor);
        }

        /// <summary>
        /// Creates a mock from several types, with strict semantics.
        /// Only <paramref name="mainType"/> may be a class.
        /// </summary>
        public object CreateMultiMock(Type mainType, params Type[] extraTypes)
        {
            return CreateMultiMock(mainType, extraTypes, new object[0]);
        }

        /// <summary>
        /// Creates a mock from several types, with strict semantics.
        /// Only <paramref name="mainType"/> may be a class.
        /// </summary>
        /// <param name="mainType">The main type to mock.</param>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class.</param>
        public object CreateMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor)
        {
            CreateMockState factory = new CreateMockState(CreateRecordState);
            return CreateMockObject(mainType, factory, extraTypes, argumentsForConstructor);
        }

        /// <summary>
        /// Creates a mock from several types, with dynamic semantics.
        /// Only <paramref name="mainType"/> may be a class.
        /// </summary>
        /// <param name="mainType">The main type to mock.</param>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        public object DynamicMultiMock(Type mainType, params Type[] extraTypes)
        {
            return DynamicMultiMock( mainType, extraTypes, new object[0] );
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
            CreateMockState factory = new CreateMockState( CreateDynamicRecordState );
            return CreateMockObject(mainType, factory, extraTypes, argumentsForConstructor);
        }

        /*
         * Method: DynamicMock
         * Create a mock object with dynamic semantics.
         * Dynamic semantics means that any call that wasn't explicitly recorded is accepted and a
         * null or zero is returned (if there is a return value).
         */
        /// <summary>
        /// Creates a dynamic mock for the specified type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
        public object DynamicMock(Type type, params object[] argumentsForConstructor)
        {
            return DynamicMultiMock(type, new Type[0], argumentsForConstructor);
        }

        /*
		 * Method: PartialMock
		 * Create a mock object with from a class that defaults to calling the class methods
         * if no expectation is set on the method.
         * 
		 */
        /// <summary>
        /// Creates a mock object that defaults to calling the class methods.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor.</param>
        public object PartialMock(Type type, params object[] argumentsForConstructor)
        {
            return PartialMultiMock(type, new Type[0], argumentsForConstructor);
        }

        /// <summary>
        /// Creates a mock object that defaults to calling the class methods.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        public object PartialMultiMock(Type type, params Type[] extraTypes)
        {
            return PartialMultiMock(type, extraTypes, new object[0]);
        }

        /// <summary>
        /// Creates a mock object that defaults to calling the class methods.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="extraTypes">Extra interface types to mock.</param>
        /// <param name="argumentsForConstructor">Arguments for the class' constructor.</param>
        public object PartialMultiMock(Type type, Type[] extraTypes, params object[] argumentsForConstructor)
        {
            if (type.IsInterface)
                throw new InvalidOperationException("Can't create a partial mock from an interface");
            CreateMockState factory = new CreateMockState(CreatePartialRecordState);
            return CreateMockObject(type, factory, extraTypes, argumentsForConstructor);
        }

        /*
         * Method: Replay
         * Moves a single mock object to the replay state.
         * This method *cannot* be called from inside an ordering.
         */
        /// <summary>
        /// Cause the mock state to change to replay, any further call is compared to the 
        /// ones that were called in the record state.
        /// </summary>
        /// <param name="obj">the object to move to replay state</param>
        public void Replay(object obj)
        {
            NotInsideOrderring();
            IsMockObjectFromThisRepository(obj);
            ClearLastProxy(obj);
            IMockState state = proxies[obj];
            proxies[obj] = state.Replay();
        }

        /*
         * Method: BackToRecord
         * 
         * Moves the mocked object back to record state.
         * This works on mock objects regardless of state. 
         * You can (and it's recommended) to run <Verify()> before you use this method, but it's not neccecary.
         * 
         * Note:
         * This will remove all the current expectations from the mock repository.
         * 
         * 
         */
        /// <summary>
        /// Move the mocked object back to record state.
        /// Will delete all current expectations!
        /// </summary>
        public void BackToRecord(object obj)
        {
            IsMockObjectFromThisRepository(obj);
            foreach (IExpectation expectation in rootRecorder.GetAllExpectationsForProxy(obj))
            {
                rootRecorder.RemoveExpectation(expectation);
            }
            rootRecorder.RemoveAllRepeatableExpectationsForProxy(obj);
            proxies[obj] = proxies[obj].BackToRecord();
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
        internal IMethodOptions LastMethodCall(object mockedInstance)
        {
            object mock = GetMockObjectFromInvocationProxy(mockedInstance);
            IsMockObjectFromThisRepository(mock);
            return proxies[mock].LastMethodOptions;
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
                return null;
            IMockState state = proxies[proxy];
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

            return proxy == null ? invocationProxy : proxy;
        }

        private RecordMockState CreateRecordState(IMockedObject mockedObject)
        {
            return new RecordMockState(mockedObject, this);
        }

        private RecordMockState CreateDynamicRecordState(IMockedObject mockedObject)
        {
            return new RecordDynamicMockState(mockedObject, this);
        }

        private RecordMockState CreatePartialRecordState(IMockedObject mockedObject)
        {
            return new RecordPartialMockState(mockedObject, this);
        }

        private void NotInsideOrderring()
        {
            if (Recorder != rootRecorder)
                throw new InvalidOperationException("Can't start replaying because Ordered or Unordered properties were call and not yet disposed.");
        }

        private void ClearLastProxy(object obj)
        {
            if (obj == lastMockedObject)
                lastMockedObject = null;
        }

        private object MockClass(CreateMockState mockStateFactory, Type type, Type[]extras, object[] argumentsForConstructor)
        {
            if (type.IsSealed)
                throw new NotSupportedException("Can't create mocks of sealed classes");
            RhinoInterceptor interceptor = new RhinoInterceptor(this, new ProxyInstance(this));
            ArrayList types = new ArrayList();
            types.AddRange(extras);
            types.Add(typeof(IMockedObject));
            object proxy;
            try
            {
                proxy = generator.CreateClassProxy(type, (Type[]) types.ToArray(typeof (Type)),
                                                   interceptor, false, argumentsForConstructor);
            }
            catch (MissingMethodException mme)
            {
                throw new MissingMethodException("Can't find a constructor with matching arguments", mme);
            }
            catch (TargetInvocationException tie)
            {
                throw new Exception("Exception in constructor: " + tie.InnerException.ToString(), tie.InnerException);
            }
            RecordMockState value = mockStateFactory((IMockedObject)proxy);
            proxies.Add(proxy, value);
            return proxy;
        }

        private object MockInterface(CreateMockState mockStateFactory, Type type, Type[] extras)
        {
            object proxy;
            RhinoInterceptor interceptor = new RhinoInterceptor(this, new ProxyInstance(this));
            ArrayList types = new ArrayList();
            types.Add(type);
            types.AddRange(extras);
            types.Add(typeof (IMockedObject));
            proxy = generator.CreateProxy((Type[]) types.ToArray(typeof (Type)), 
                                          interceptor, new object());
            RecordMockState value = mockStateFactory((IMockedObject)proxy);
            proxies.Add(proxy, value);
            return proxy;
        }

        private object MockDelegate(CreateMockState mockStateFactory, Type type)
        {
            if (typeof(Delegate).Equals(type))
                throw new InvalidOperationException("Cannot mock the Delegate base type.");

            object proxy;

            ProxyInstance proxyInstance = new ProxyInstance(this);
            RhinoInterceptor interceptor = new RhinoInterceptor(this, proxyInstance);

            Type[] types = new Type[] { delegateTargetInterfaceCreator.GetDelegateTargetInterface(type), typeof(IMockedObject) };
            object target = generator.CreateProxy(types, interceptor, new object());

            proxy = Delegate.CreateDelegate(type, target, "Invoke");
            delegateProxies.Add(target, proxy);

            RecordMockState value = mockStateFactory(GetMockedObject(proxy));
            proxies.Add(proxy, value);
            return proxy;
        }

        private object CreateMockObject(Type type, CreateMockState factory, Type[] extras, object[] argumentsForConstructor)
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
                    throw new ArgumentException("Constructor arguments should not be supplied when mocking an interface", "argumentsForConstructor");
                }
                return MockInterface(factory, type, extras);
            }
            else if (typeof(Delegate).IsAssignableFrom(type))
            {
                if (argumentsForConstructor != null && argumentsForConstructor.Length > 0)
                {
                    throw new ArgumentException("Constructor arguments should not be supplied when mocking a delegate", "argumentsForConstructor");
                }
                return MockDelegate(factory, type);
            }
            else
                return MockClass(factory, type, extras, argumentsForConstructor);
        }

        private void IsMockObjectFromThisRepository(object obj)
        {
            if (proxies.ContainsKey(obj) == false)
                throw new ObjectNotMockFromThisRepositoryException("The object is not a mock object that belong to this repository.");
        }

        /*
         * Method: GetMockedObject
         * Get an IProxy from a mocked object instance, or throws if the 
         * object is not a mock object.
         */
        internal static IMockedObject GetMockedObject(object mockedInstance)
        {
            IMockedObject mockedObj = GetMockedObjectOrNull(mockedInstance);
            if (mockedObj == null)
                throw new InvalidOperationException("The object '" + mockedInstance.ToString() + "' is not a mocked object.");
            return mockedObj;
        }

        /*
         * Method: GetMockedObjectOrNull
         * Get an IProxy from a mocked object instance, or null if the
         * object is not a mock object.
         */
        internal static IMockedObject GetMockedObjectOrNull(object mockedInstance)
        {
            Delegate mockedDelegate = mockedInstance as Delegate;

            if (mockedDelegate != null)
                mockedInstance = mockedDelegate.Target;

            IMockedObject mockedObj = mockedInstance as IMockedObject;
            return mockedObj;
        }

        /// <summary>
        /// Pops the recorder.
        /// </summary>
        internal void PopRecorder()
        {
            if (recorders.Count > 1)
                recorders.Pop();
        }

        /// <summary>
        /// Pushes the recorder.
        /// </summary>
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
			if (proxies.Count == 0)
				return;
			foreach (object key in new ArrayList(proxies.Keys))
			{
				if (proxies[key] is RecordMockState)
					BackToRecord(key);
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
         *	using(MockRepository mocks = new MockRepository())
         *	{
         *		// Some action that cause an unexpected exception
         *		// which would cause unsatisfied expectation and cause
         *		// VerifyAll() to fail.
         *	}
         * (end)
         *
         * The unrolled using statement:
         * (start code)
         *	MockRepository mocks = new MockRepository())
         *	//The afore mentioned action
         *	mocks.VerifyAll()//won't occur if an exception is thrown
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
            foreach (object key in new ArrayList(proxies.Keys))
            {
                if (proxies[key] is VerifiedMockState)
                    continue;
                Verify(key);
            }
        }

        /*
         * Property: Replayer
         * The replayer for the repository.
         */
        /// <summary>
        /// Gets the replayer for this repository.
        /// </summary>
        /// <value></value>
        internal IMethodRecorder Replayer
        {
            get { return rootRecorder; }
        }

        /*
         * Property: LastProxy
         * The last mock object that was called on *any* repository.
         */
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

        #endregion
    }
}
