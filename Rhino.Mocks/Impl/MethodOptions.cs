using System;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Interfaces;
using System.Reflection;
using System.Collections;

namespace Rhino.Mocks.Impl
{
    /// <summary>
    /// Allows to define what would happen when a method 
    /// is called.
    /// </summary>
    public class MethodOptions : IMethodOptions, IRepeat
    {
        #region Variables

        private IExpectation expectation;
        private bool expectationReplaced = false;
        private MockRepository repository;
        private readonly RecordMockState record;
        private IMockedObject proxy;

        #endregion

        #region Properties

        /// <summary>
        /// Better syntax to define repeats. 
        /// </summary>
        public IRepeat Repeat
        {
            get { return this; }
        }

        #endregion

        #region C'tor

        /// <summary>
        /// Creates a new <see cref="MethodOptions"/> instance.
        /// </summary>
        /// <param name="repository">the repository for this expectation</param>
        /// <param name="record">the recorder for this proxy</param>
        /// <param name="proxy">the proxy for this expectation</param>
        /// <param name="expectation">Expectation.</param>
        public MethodOptions(MockRepository repository, RecordMockState record, IMockedObject proxy, IExpectation expectation)
        {
            this.expectation = expectation;
            this.proxy = proxy;
            this.repository = repository;
            this.record = record;

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add constraints for the method's arguments.
        /// </summary>
        public IMethodOptions Constraints(params AbstractConstraint[] constraints)
        {
            ConstraintsExpectation constraintsExpectation = new ConstraintsExpectation(expectation, constraints);
            ReplaceExpectation(constraintsExpectation);
            return this;
        }

        /// <summary>
        /// Set a callback method for the last call
        /// </summary>
        public IMethodOptions Callback(Delegate callback)
        {
            CallbackExpectation callbackExpectation = new CallbackExpectation(expectation, callback);
            ReplaceExpectation(callbackExpectation);
            return this;
        }


        /// <summary>
        /// Set a delegate to be called when the expectation is matched.
        /// The delegate return value will be returned from the expectation.
        /// </summary>
        public IMethodOptions Do(Delegate action)
        {
            expectation.ActionToExecute = action;
            return this;
        }


        /// <summary>
        /// Set the return value for the method.
        /// </summary>
        /// <param name="objToReturn">The object the method will return</param>
        /// <returns>IRepeat that defines how many times the method will return this value</returns>
        public IMethodOptions Return(object objToReturn)
        {
            expectation.ReturnValue = objToReturn;
            return this;
        }

        /// <summary>
        /// Throws the specified exception when the method is called.
        /// </summary>
        /// <param name="exception">Exception to throw</param>
        public IMethodOptions Throw(Exception exception)
        {
            expectation.ExceptionToThrow = exception;
            return this;
        }

        /// <summary>
        /// Ignores the arguments for this method. Any argument will be matched
        /// againt this method.
        /// </summary>
        public IMethodOptions IgnoreArguments()
        {
            AnyArgsExpectation anyArgsExpectation = new AnyArgsExpectation(expectation);
            ReplaceExpectation(anyArgsExpectation);
            return this;
        }

        /// <summary>
        /// Call the original method on the class, bypassing the mocking layers.
        /// </summary>
        /// <returns></returns>
        public void CallOriginalMethod()
        {
            AssertMethodImplementationExists();
            proxy.RegisterMethodForCallingOriginal(expectation.Method);
            repository.Recorder.RemoveExpectation(expectation);
            expectation.RepeatableOption = RepeatableOption.OriginalCall;
        }

        /// <summary>
        /// Use the property as a simple property, getting/setting the values without
        /// causing mock expectations.
        /// </summary>
        public IMethodOptions PropertyBehavior()
        {
            AssertExpectationOnPropertyWithGetterAndSetter();
            PropertyInfo prop = GetPropertyFromMethod(expectation.Method);
            proxy.RegisterPropertyBehaviorFor(prop);
            repository.Recorder.RemoveExpectation(expectation);
            expectation.RepeatableOption = RepeatableOption.PropertyBehavior;
        	return this;
        }

        /// <summary>
        /// Gets the event raiser for the last event
        /// </summary>
        public IEventRaiser GetEventRaiser()
        {
            AssertLastMethodWasEventAddOrRemove();
            string eventName = expectation.Method.Name.StartsWith("add_") ?
                expectation.Method.Name.Substring(4) : expectation.Method.Name.Substring(7);
            return new EventRaiser(proxy, eventName);
        }

        /// <summary>
        /// Set the parameter values for out and ref parameters.
        /// This is done using zero based indexing, and _ignoring_ any non out/ref parameter.
        /// </summary>
        public IMethodOptions OutRef(params object[] parameters)
        {
            Validate.IsNotNull(parameters,"parameters");
            expectation.OutRefParams = parameters;
            return this;
        }

        #endregion

        #region Implementation

        private void AssertExpectationOnPropertyWithGetterAndSetter()
        {
            MethodInfo method = expectation.Method;
            //not a property geter or setter
            if (false == (method.IsSpecialName &&
                (method.Name.StartsWith("get_") ||
                method.Name.StartsWith("set_"))))
                throw new InvalidOperationException("Last method call was not made on a setter or a getter");
            PropertyInfo prop = GetPropertyFromMethod(method);
            if (false == (prop.CanRead && prop.CanWrite))
                throw new InvalidOperationException("Property must be read/write");
        }

        private void AssertLastMethodWasEventAddOrRemove()
        {
            MethodInfo method = expectation.Method;
            if (method.IsSpecialName == false ||
                !(method.Name.StartsWith("add_") || method.Name.StartsWith("remove_")))
            {
                throw new InvalidOperationException("The last method call " + method.Name + " was not an event add / remove method");
            }
        }

        private PropertyInfo GetPropertyFromMethod(MethodInfo method)
        {
            string propName = method.Name.Substring(4);
            ParameterInfo[] args = method.GetParameters();
            ArrayList types = new ArrayList();
            for (int i = 0; i < args.Length; i++)
			{
                //remove the value parameter for finding the property if indexed
                if(i==0 && method.Name.StartsWith("set_"))
                    continue;
                types.Add(args[i].ParameterType);
			}
            PropertyInfo prop = expectation.Method.DeclaringType.GetProperty(propName,
                (Type[])types.ToArray(typeof(Type)));
            return prop;
        }

        private void AssertMethodImplementationExists()
        {
            if (expectation.Method.IsAbstract)
                throw new InvalidOperationException("Can't use CallOriginalMethod on method " + expectation.Method.Name + " because the method is abstract.");
        }

        private void ReplaceExpectation(IExpectation anyArgsExpectation)
        {
            //All other expectations ignore arguments, so it's safe to replace 
            //arguments when the previous expectation is any args.
            if (expectationReplaced && !(expectation is AnyArgsExpectation))
            {
                string message = "This method has already been set to " + expectation.GetType().Name + ".";
                throw new InvalidOperationException(message);
            }
            repository.Recorder.ReplaceExpectation(proxy, expectation.Method, expectation, anyArgsExpectation);
            expectation = anyArgsExpectation;
            record.LastExpectation = expectation;
            expectationReplaced = true;
        }

        #endregion

        #region IRepeat Implementation

        /// <summary>
        /// Repeat the method twice.
        /// </summary>
        public IMethodOptions Twice()
        {
            expectation.Expected = new Range(2, 2);
            expectation.RepeatableOption = RepeatableOption.Normal;
            return this;
        }

        /// <summary>
        /// Repeat the method once.
        /// </summary>
        public IMethodOptions Once()
        {
            expectation.Expected = new Range(1, 1);
            expectation.RepeatableOption = RepeatableOption.Normal;
            return this;
        }

        /// <summary>
        /// Repeat the method at least once, then repeat as many time as it would like.
        /// </summary>
        public IMethodOptions AtLeastOnce()
        {
            expectation.Expected = new Range(1, int.MaxValue);
            expectation.RepeatableOption = RepeatableOption.Normal;
            return this;
        }

        /// <summary>
        /// This method must not appear in the replay state.
        /// </summary>
        public IMethodOptions Never()
        {
            expectation.Expected = new Range(0, 0);
            //This expectation will not be thrown, but it will
            //make sure that ExpectationSatisfied will be true if
            //the method under consideration has a return value;
            expectation.ExceptionToThrow = new InvalidOperationException("This is a method that should not be called");
            expectation.RepeatableOption = RepeatableOption.Never;
            repository.Replayer.AddToRepeatableMethods(proxy, expectation.Method, expectation);
            return this;
        }
    	
    	/// <summary>
    	/// Documentation message for the expectation
    	/// </summary>
    	/// <param name="documentationMessage">Message</param>
    	public IMethodOptions Message(string documentationMessage)
    	{
			expectation.Message = documentationMessage;
    		return this;
    	}

        /// <summary>
        /// Repeat the method any number of times.
        /// </summary>
        public IMethodOptions Any()
        {
            expectation.Expected = new Range(int.MaxValue, int.MaxValue);
            expectation.RepeatableOption = RepeatableOption.Any;
            repository.Replayer.AddToRepeatableMethods(proxy, expectation.Method, expectation);
            return this;
        }

        /// <summary>
        /// Set the range to repeat an action.
        /// </summary>
        /// <param name="min">Min.</param>
        /// <param name="max">Max.</param>
        public IMethodOptions Times(int min, int max)
        {
            expectation.Expected = new Range(min, max);
            expectation.RepeatableOption = RepeatableOption.Normal;
            return this;
        }

        /// <summary>
        /// Set the amount of times to repeat an action.
        /// </summary>
        public IMethodOptions Times(int count)
        {
            expectation.Expected = new Range(count, count);
            expectation.RepeatableOption = RepeatableOption.Normal;
            return this;

        }

        #endregion


    }
}