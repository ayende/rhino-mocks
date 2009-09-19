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
	public class MethodOptions<T> : IMethodOptions<T>, IRepeat<T>
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
		public IRepeat<T> Repeat
		{
			get { return this; }
		}

		#endregion

		#region C'tor

		/// <summary>
		/// Creates a new <see cref="IMethodOptions{T}"/> instance.
		/// </summary>
		/// <param name="repository">the repository for this expectation</param>
		/// <param name="record">the recorder for this proxy</param>
		/// <param name="proxy">the proxy for this expectation</param>
		/// <param name="expectation">Expectation.</param>
		public MethodOptions(
			MockRepository repository, 
			RecordMockState record, 
			IMockedObject proxy, 
			IExpectation expectation)
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
		public IMethodOptions<T> Constraints(params AbstractConstraint[] constraints)
		{
			if (expectation is ConstraintsExpectation)
			{
				throw new InvalidOperationException(
					string.Format("You have already specified constraints for this method. ({0})", this.expectation.ErrorMessage));
			}
			ConstraintsExpectation constraintsExpectation = new ConstraintsExpectation(expectation, constraints);
			ReplaceExpectation(constraintsExpectation);
			return this;
		}

		/// <summary>
		/// Set a callback method for the last call
		/// </summary>
		public IMethodOptions<T> Callback(Delegate callback)
		{
			CallbackExpectation callbackExpectation = new CallbackExpectation(expectation, callback);
			ReplaceExpectation(callbackExpectation);
			return this;
		}

		/// <summary>
		/// Set a callback method for the last call
		/// </summary>
		public IMethodOptions<T> Callback(Delegates.Function<bool> callback)
		{
			return Callback((Delegate) callback);
		}

		/// <summary>
		/// Set a callback method for the last call
		/// </summary>
		public IMethodOptions<T> Callback<TArg0>(Delegates.Function<bool, TArg0> callback)
		{
			return Callback( (Delegate) callback);
		}

		/// <summary>
		/// Set a callback method for the last call
		/// </summary>
		public IMethodOptions<T> Callback<TArg0, TArg1>(Delegates.Function<bool, TArg0, TArg1> callback)
		{
			return Callback( (Delegate) callback);
		}

		/// <summary>
		/// Set a callback method for the last call
		/// </summary>
		public IMethodOptions<T> Callback<TArg0, TArg1, TArg2>(Delegates.Function<bool, TArg0, TArg1, TArg2> callback)
		{
			return Callback( (Delegate) callback);
		}

		/// <summary>
		/// Set a callback method for the last call
		/// </summary>
		public IMethodOptions<T> Callback<TArg0, TArg1, TArg2, TArg3>(
			Delegates.Function<bool, TArg0, TArg1, TArg2, TArg3> callback)
		{
			return Callback( (Delegate) callback);
		}

		/// <summary>
		/// Set a callback method for the last call
		/// </summary>
		public IMethodOptions<T> Callback<TArg0, TArg1, TArg2, TArg3, TArg4>(
			Delegates.Function<bool, TArg0, TArg1, TArg2, TArg3, TArg4> callback)
		{
			return Callback( (Delegate) callback);
		}

		/// <summary>
		/// Set a callback method for the last call
		/// </summary>
		public IMethodOptions<T> Callback<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5>(
			Delegates.Function<bool, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5> callback)
		{
			return Callback( (Delegate) callback);
		}

		/// <summary>
		/// Set a callback method for the last call
		/// </summary>
		public IMethodOptions<T> Callback<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(
			Delegates.Function<bool, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> callback)
		{
			return Callback( (Delegate) callback);
		}

		/// <summary>
		/// Set a callback method for the last call
		/// </summary>
		public IMethodOptions<T> Callback<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(
			Delegates.Function<bool, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> callback)
		{
			return Callback( (Delegate) callback);
		}

		/// <summary>
		/// Set a callback method for the last call
		/// </summary>
		public IMethodOptions<T> Callback<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(
			Delegates.Function<bool, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> callback)
		{
			return Callback( (Delegate) callback);
		}

		/// <summary>
		/// Set a callback method for the last call
		/// </summary>
		public IMethodOptions<T> Callback<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(
			Delegates.Function<bool, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> callback)
		{
			return Callback( (Delegate) callback);
		}


		/// <summary>
		/// Set a delegate to be called when the expectation is matched.
		/// The delegate return value will be returned from the expectation.
		/// </summary>
		public IMethodOptions<T> Do(Delegate action)
		{
			expectation.ActionToExecute = action;
			return this;
		}


		/// <summary>
		/// Set a delegate to be called when the expectation is matched.
		/// The delegate return value will be returned from the expectation.
		/// </summary>
		public IMethodOptions<T> WhenCalled(Action<MethodInvocation> action)
		{
			expectation.WhenCalled += action;
			return this;
		}


		/// <summary>
		/// Set the return value for the method.
		/// </summary>
		/// <param name="objToReturn">The object the method will return</param>
		/// <returns>IRepeat that defines how many times the method will return this value</returns>
		public IMethodOptions<T> Return(T objToReturn)
		{
			expectation.ReturnValue = objToReturn;
			return this;
		}


        /// <summary>
        /// Set the return value for the method, but allow to override this return value in the future
        /// </summary>
        /// <returns>IRepeat that defines how many times the method will return this value</returns>
        public IMethodOptions<T> TentativeReturn()
	    {
            expectation.AllowTentativeReturn = true;
            return this;
	    }

	    /// <summary>
		/// Throws the specified exception when the method is called.
		/// </summary>
		/// <param name="exception">Exception to throw</param>
		public IMethodOptions<T> Throw(Exception exception)
		{
			expectation.ExceptionToThrow = exception;
			return this;
		}

		/// <summary>
		/// Ignores the arguments for this method. Any argument will be matched
		/// againt this method.
		/// </summary>
		public IMethodOptions<T> IgnoreArguments()
		{
			AnyArgsExpectation anyArgsExpectation = new AnyArgsExpectation(expectation);
			ReplaceExpectation(anyArgsExpectation);
			return this;
		}

		/// <summary>
		/// Call the original method on the class, bypassing the mocking layers.
		/// </summary>
		/// <returns></returns>
		[Obsolete("Use CallOriginalMethod(OriginalCallOptions options) overload to explicitly specify the call options")]
		public void CallOriginalMethod()
		{
			CallOriginalMethod(OriginalCallOptions.NoExpectation);
		}


		/// <summary>
		/// Call the original method on the class, optionally bypassing the mocking layers
		/// </summary>
		/// <returns></returns>
		public IMethodOptions<T> CallOriginalMethod(OriginalCallOptions options)
		{
			AssertMethodImplementationExists();
			if (options == OriginalCallOptions.NoExpectation)
			{
				proxy.RegisterMethodForCallingOriginal(expectation.Method);
				repository.Recorder.RemoveExpectation(expectation);
				expectation.RepeatableOption = RepeatableOption.OriginalCallBypassingMocking;
			}
			else
			{
				expectation.RepeatableOption = RepeatableOption.OriginalCall;
			}
			return this;
		}


		/// <summary>
		/// Use the property as a simple property, getting/setting the values without
		/// causing mock expectations.
		/// </summary>
		public IMethodOptions<T> PropertyBehavior()
		{
			AssertExpectationOnPropertyWithGetterAndSetter();
			PropertyInfo prop = GetPropertyFromMethod(expectation.Method);
			proxy.RegisterPropertyBehaviorFor(prop);
			repository.Recorder.RemoveExpectation(expectation);
			expectation.RepeatableOption = RepeatableOption.PropertyBehavior;
			return this;
		}

		/// <summary>
		/// Expect last (property) call as property setting, ignore the argument given
		/// </summary>
		/// <returns></returns>
        public IMethodOptions<T> SetPropertyAndIgnoreArgument()
        {
			bool isInReplayMode = repository.IsInReplayMode(proxy);
			if(isInReplayMode)
				repository.BackToRecord(proxy,BackToRecordOptions.None);
            expectation.ReturnValue = default(T);
            MethodInfo setter = PropertySetterFromMethod(expectation.Method);
            repository.Recorder.RemoveExpectation(expectation);
            setter.Invoke(proxy, new object[] {default(T)});
			IMethodOptions<T> methodOptions = repository.LastMethodCall<T>(proxy).IgnoreArguments();
			if (isInReplayMode)
				repository.ReplayCore(proxy,true);
			return methodOptions;
        }

		/// <summary>
		/// Expect last (property) call as property setting with a given argument.
		/// </summary>
		/// <param name="argument"></param>
		/// <returns></returns>
        public IMethodOptions<T> SetPropertyWithArgument(T argument)
        {
			bool isInReplayMode = repository.IsInReplayMode(proxy);
			if (isInReplayMode)
				repository.BackToRecord(proxy, BackToRecordOptions.None);
			expectation.ReturnValue = default(T);
            MethodInfo setter = PropertySetterFromMethod(expectation.Method);
            repository.Recorder.RemoveExpectation(expectation);
            setter.Invoke(proxy, new object[] { argument });
			IMethodOptions<T> methodOptions = repository.LastMethodCall<T>(proxy);
			if (isInReplayMode)
				repository.ReplayCore(proxy, true);
			return methodOptions;
        }

		/// <summary>
		/// Gets the event raiser for the last event
		/// </summary>
		public IEventRaiser GetEventRaiser()
		{
			AssertLastMethodWasEventAddOrRemove();
			string eventName = expectation.Method.Name.StartsWith("add_")
			                   	?
			                   		expectation.Method.Name.Substring(4)
			                   	: expectation.Method.Name.Substring(7);
			return new EventRaiser(proxy, eventName);
		}

		/// <summary>
		/// Set the parameter values for out and ref parameters.
		/// This is done using zero based indexing, and _ignoring_ any non out/ref parameter.
		/// </summary>
		public IMethodOptions<T> OutRef(params object[] parameters)
		{
			Validate.IsNotNull(parameters, "parameters");
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
			if (!(method.Name.StartsWith("add_") || method.Name.StartsWith("remove_")))
			{
				throw new InvalidOperationException("The last method call " + method.Name + " was not an event add / remove method");
			}
		}

        private MethodInfo PropertySetterFromMethod(MethodInfo method)
        {
            //not a property geter or setter
            if (false == (method.IsSpecialName &&
                          (method.Name.StartsWith("get_") ||
                           method.Name.StartsWith("set_"))))
                throw new InvalidOperationException("Last method call was not made on a setter or a getter");
            PropertyInfo prop = GetPropertyFromMethod(method);
            if (!prop.CanWrite)
                throw new InvalidOperationException("Property must be writeable");

            return method.DeclaringType.GetMethod("set_" + method.Name.Substring(4));
        }

		private PropertyInfo GetPropertyFromMethod(MethodInfo method)
		{
			string propName = method.Name.Substring(4);
			ParameterInfo[] args = method.GetParameters();
			ArrayList types = new ArrayList();
			for (int i = 0; i < args.Length; i++)
			{
				//remove the value parameter for finding the property if indexed
				if (i == 0 && method.Name.StartsWith("set_"))
					continue;
				types.Add(args[i].ParameterType);
			}
			PropertyInfo prop = expectation.Method.DeclaringType.GetProperty(propName,
			                                                                 (Type[]) types.ToArray(typeof (Type)));
			return prop;
		}

		private void AssertMethodImplementationExists()
		{
			if (expectation.Method.IsAbstract)
				throw new InvalidOperationException("Can't use CallOriginalMethod on method " + expectation.Method.Name +
				                                    " because the method is abstract.");
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
		public IMethodOptions<T> Twice()
		{
			expectation.Expected = new Range(2, 2);
			return this;
		}

		/// <summary>
		/// Repeat the method once.
		/// </summary>
		public IMethodOptions<T> Once()
		{
			expectation.Expected = new Range(1, 1);
			return this;
		}

		/// <summary>
		/// Repeat the method at least once, then repeat as many time as it would like.
		/// </summary>
		public IMethodOptions<T> AtLeastOnce()
		{
			expectation.Expected = new Range(1, int.MaxValue);
			return this;
		}

		/// <summary>
		/// This method must not appear in the replay state.
		/// </summary>
		public IMethodOptions<T> Never()
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
		public IMethodOptions<T> Message(string documentationMessage)
		{
			expectation.Message = documentationMessage;
			return this;
		}

		/// <summary>
		/// Repeat the method any number of times.
		/// </summary>
		public IMethodOptions<T> Any()
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
		public IMethodOptions<T> Times(int min, int max)
		{
			expectation.Expected = new Range(min, max);
			return this;
		}

		/// <summary>
		/// Set the amount of times to repeat an action.
		/// </summary>
		public IMethodOptions<T> Times(int count)
		{
			expectation.Expected = new Range(count, count);
			return this;
		}

		#endregion
	}
}
