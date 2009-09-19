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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Castle.Core.Interceptor;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Expectations
{
    /// <summary>
    /// Abstract class that holds common information for 
    /// expectations.
    /// </summary>
    public abstract class AbstractExpectation : IExpectation
    {
        #region Variables

        /// <summary>
        /// Number of actuall calls made that passed this expectation
        /// </summary>
        private int actualCallsCount;

        /// <summary>
        /// Range of expected calls that should pass this expectation.
        /// </summary>
        private Range expected;

        /// <summary>
        /// The return value for a method matching this expectation
        /// </summary>
        private object returnValue;

        /// <summary>
        /// The exception to throw on a method matching this expectation.
        /// </summary>
        private Exception exceptionToThrow;

        /// <summary>
        /// The method this expectation is for.
        /// </summary>
        private MethodInfo method;

        /// <summary>
        /// The return value for this method was set
        /// </summary>
        private bool returnValueSet;

        /// <summary>
        /// Whether this method will repeat
        /// unlimited number of times.
        /// </summary>
        private RepeatableOption repeatableOption = RepeatableOption.Normal;

        /// <summary>
        /// A delegate that will be run when the 
        /// expectation is matched.
        /// </summary>
        private Delegate actionToExecute;

        /// <summary>
        /// The arguments that matched this expectation.
        /// </summary>
        private object[] matchingArgs;

        private object[] outRefParams;

        /// <summary>
        /// Documentation message
        /// </summary>
        private string message;

        /// <summary>
        /// The method originalInvocation
        /// </summary>
        private readonly IInvocation originalInvocation;

        private bool allowTentativeReturn = false;

        #endregion

        #region Properties

        /// <summary>
        /// Setter for the outpur / ref parameters for this expecataion.
        /// Can only be set once.
        /// </summary>
        public object[] OutRefParams
        {
			get
			{
				return outRefParams;
			}
            set
            {
                if (outRefParams != null)
                    throw new InvalidOperationException(
                        "Output and ref parameters has already been set for this expectation");
                outRefParams = value;
            }
        }

        /// <summary>
        /// Specify whether this expectation has a return value set
        /// You can't check ReturnValue for this because a valid return value include null.
        /// </summary>
        public bool HasReturnValue
        {
            get { return returnValueSet; }
        }

        /// <summary>
        /// Gets the method this expectation is for.
        /// </summary>
        public MethodInfo Method
        {
            get { return method; }
        }


        /// <summary>
        /// Gets the originalInvocation for this expectation
        /// </summary>
        /// <value>The originalInvocation.</value>
        public IInvocation Originalinvocation
        {
            get { return originalInvocation; }
        }

        /// <summary>
        /// Gets or sets what special condtions there are for this method
        /// </summary>
        public RepeatableOption RepeatableOption
        {
            get { return repeatableOption; }
            set { repeatableOption = value; }
        }

        /// <summary>
        /// Range of expected calls
        /// </summary>
        public Range Expected
        {
            get { return expected; }
            set { expected = value; }
        }

        /// <summary>
        /// Number of call actually made for this method
        /// </summary>
        public int ActualCallsCount
        {
            get { return actualCallsCount; }
        }

        /// <summary>
        /// If this expectation is still waiting for calls.
        /// </summary>
        public bool CanAcceptCalls
        {
            get
            {
                //I don't bother to check for RepeatableOption.Never because
                //this is handled the method recorder
                if (repeatableOption == RepeatableOption.Any)
                    return true;
                return expected.Max == null || actualCallsCount < expected.Max.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this expectation was satisfied
        /// </summary>
        public bool ExpectationSatisfied
        {
            get
            {
                if (repeatableOption != RepeatableOption.Normal && repeatableOption != RepeatableOption.OriginalCall)
                    return true;
                if(expected.Min > actualCallsCount )
                    return false;
                if(Expected.Max==null)
                    return true;
                return actualCallsCount <= expected.Max.Value;
            }
        }


        /// <summary>
        /// The return value for a method matching this expectation
        /// </summary>
        public object ReturnValue
        {
            get { return returnValue; }
            set
            {
                ActionOnMethodNotSpesified();
                AssertTypesMatches(value);
                returnValueSet = true;
                returnValue = value;
            }
        }

        /// <summary>
        /// An action to execute when the method is matched.
        /// </summary>
        public Delegate ActionToExecute
        {
        	get { return actionToExecute; }
        	set
            {
                ActionOnMethodNotSpesified();
                AssertReturnTypeMatch(value);
                AssertDelegateArgumentsMatchMethod(value);
                actionToExecute = value;
            }
        }

    	/// <summary>
        /// Gets or sets the exception to throw on a method matching this expectation.
        /// </summary>
        public Exception ExceptionToThrow
        {
            get { return exceptionToThrow; }
            set
            {
                ActionOnMethodNotSpesified();
                exceptionToThrow = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance's action is staisfied.
        /// A staisfied instance means that there are no more requirements from
        /// this method. A method with non void return value must register either
        /// a return value or an exception to throw or an action to execute.
        /// </summary>
        public bool ActionsSatisfied
        {
            get
            {
                if (method.ReturnType == typeof(void) ||
                    exceptionToThrow != null ||
                    actionToExecute != null ||
                    returnValueSet ||
                    allowTentativeReturn ||
                    repeatableOption == RepeatableOption.OriginalCall ||
                    repeatableOption == RepeatableOption.OriginalCallBypassingMocking ||
                    repeatableOption == RepeatableOption.PropertyBehavior)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Documentation message
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the hash code
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Add an actual actualMethodCall call to this expectation
        /// </summary>
        public void AddActualCall()
        {
            actualCallsCount++;
        }

		/// <summary>
		/// Builds the verification failure message.
		/// </summary>
		/// <returns></returns>
    	public string BuildVerificationFailureMessage()
    	{
			StringBuilder sb = new StringBuilder();
			string expectationMessege = ErrorMessage;
			sb.Append(expectationMessege).Append(' ');
			sb.Append("Expected #");
			sb.Append(Expected.ToString()).Append(", ");
			sb.Append("Actual #").Append(ActualCallsCount).Append('.');
			return sb.ToString();
    	}

		/// <summary>
		/// Occurs when the exceptation is match on a method call
		/// </summary>
    	public event Action<MethodInvocation> WhenCalled =delegate { };

    	/// <summary>
        /// Returns the return value or throw the exception and setup output / ref parameters
        /// </summary>
        public object ReturnOrThrow(IInvocation invocation, object[] args)
        {
            allowTentativeReturn = false;

            if (ActionsSatisfied == false)
                throw new InvalidOperationException("Method '" + ErrorMessage + "' requires a return value or an exception to throw.");

            SetupOutputAndRefParameters(args);
            if (actionToExecute != null)
            {
            	object action = ExecuteAction();
				WhenCalled(new MethodInvocation(invocation));
            	return action;
            }
    		if (exceptionToThrow != null)
                throw exceptionToThrow;
            if (RepeatableOption == RepeatableOption.OriginalCall)
            {
                invocation.Proceed();
				WhenCalled(new MethodInvocation(invocation));
				return invocation.ReturnValue;
            }
    		invocation.ReturnValue = returnValue;
			WhenCalled(new MethodInvocation(invocation));
			return invocation.ReturnValue;
        }

        /// <summary>
        /// Validate the arguments for the method on the child methods
        /// </summary>
        /// <param name="args">The arguments with which the method was called</param>
        public bool IsExpected(object[] args)
        {
            if (DoIsExpected(args))
            {
                matchingArgs = args;
                return true;
            }
            matchingArgs = null;
            return false;
        }


        #endregion

        #region C'tor

        /// <summary>
        /// Creates a new <see cref="AbstractExpectation"/> instance.
        /// </summary>
        /// <param name="invocation">The originalInvocation for this method, required because it contains the generic type infromation</param>
        /// <param name="expectedRange">Number of method calls for this expectations</param>
		protected AbstractExpectation(IInvocation invocation, Range expectedRange)
        {
            Validate.IsNotNull(invocation, "originalInvocation");
            Validate.IsNotNull(invocation.Method, "method");
            this.originalInvocation = invocation;
            this.method = invocation.Method;
            this.expected = expectedRange;
        }

        /// <summary>
        /// Creates a new <see cref="AbstractExpectation"/> instance.
        /// </summary>
        /// <param name="expectation">Expectation.</param>
        protected AbstractExpectation(IExpectation expectation)
            : this(expectation.Originalinvocation, new Range(1, 1))
        {
            returnValue = expectation.ReturnValue;
            returnValueSet = expectation.HasReturnValue;
            expected = expectation.Expected;
            actualCallsCount = expectation.ActualCallsCount;
            repeatableOption = expectation.RepeatableOption;
            exceptionToThrow = expectation.ExceptionToThrow;
            message = expectation.Message;
        	actionToExecute = expectation.ActionToExecute;
        	outRefParams = expectation.OutRefParams;
            allowTentativeReturn = expectation.AllowTentativeReturn;
        }

        /// <summary>
        /// Allow to set the return value in the future, if it was already set.
        /// </summary>
        public bool AllowTentativeReturn
        {
            get { return allowTentativeReturn; }
            set { allowTentativeReturn = value; }
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Validate the arguments for the method on the child methods
        /// </summary>
        /// <param name="args">The arguments with which the method was called</param>
        protected abstract bool DoIsExpected(object[] args);

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value></value>
        public abstract string ErrorMessage { get; }

        /// <summary>
        /// Determines if this object equal to obj
        /// </summary>
        public abstract override bool Equals(object obj);

        #endregion

        #region Protected Methods

        /// <summary>
        /// The error message for these arguments
        /// </summary>
        protected string CreateErrorMessage(string derivedMessage)
        {
            if (Message == null)
                return derivedMessage;
            string msg = string.Format("Message: {0}\n{1}", Message, derivedMessage);
            return msg;
        }

        #endregion

        #region Implementation

        private void SetupOutputAndRefParameters(object[] args)
        {
            if (outRefParams == null)
                return;
            int argIndex = 0, outputArgIndex = 0;
            ParameterInfo[] parameters = method.GetParameters();
            foreach (ParameterInfo info in parameters)
            {
                if (outputArgIndex >= outRefParams.Length)
                    return;

                if (info.IsOut || info.ParameterType.IsByRef)
                {
                    args[argIndex] = outRefParams[outputArgIndex];
                    outputArgIndex += 1;
                }
                argIndex++;
            }
        }

        private void ActionOnMethodNotSpesified()
        {
            if(allowTentativeReturn)
                return;
            if (returnValueSet == false && exceptionToThrow == null && actionToExecute == null)
                return;
            if (this.Expected != null && this.Expected.Max == 0)// we have choosen Repeat.Never
            {
                throw new InvalidOperationException(
                    "After specifying Repeat.Never(), you cannot specify a return value, exception to throw or an action to execute");
            }
            throw new InvalidOperationException("Can set only a single return value or exception to throw or delegate to execute on the same method call.");
        }

        private object ExecuteAction()
        {
            try
            {
                if (matchingArgs == null)
                    throw new InvalidOperationException("Trying to run a Do() delegate when no arguments were matched to the expectation.");
                try
                {
                    return actionToExecute.DynamicInvoke(matchingArgs);
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            }
            finally
            {
                matchingArgs = null;
            }
        }

        private void AssertTypesMatches(object value)
        {
            string type = null;
            Type returnType = method.ReturnType;
            if (value == null)
            {
                type = "null";
                if (returnType.IsValueType == false)
                    return;
                if (returnType.IsGenericType &&
                    returnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return;
            }
            else
            {
                //we reduce checking of generic types because of the complexity,
                //we let the runtime catch those mistakes
                returnType = GenericsUtil.GetRealType(returnType, originalInvocation);
                Type valueType = value.GetType();
                if (returnType.IsInstanceOfType(value))
                    return;
                type = valueType.FullName;
            }
            throw new InvalidOperationException(string.Format("Type '{0}' doesn't match the return type '{1}' for method '{2}'", type, returnType,
                MethodCallUtil.StringPresentation(Originalinvocation, method, new object[0])));
        }

        /// <summary>
        /// Asserts that the delegate has the same parameters as the expectation's method call
        /// </summary>
        protected void AssertDelegateArgumentsMatchMethod(Delegate callback)
        {
            ParameterInfo[] callbackParams = callback.Method.GetParameters(),
                         methodParams = method.GetParameters();
            string argsDontMatch = "Callback arguments didn't match the method arguments";
            if (callbackParams.Length != methodParams.Length)
                throw new InvalidOperationException(argsDontMatch);
            for (int i = 0; i < methodParams.Length; i++)
            {
                Type methodParameter = GenericsUtil.GetRealType(methodParams[i].ParameterType, Originalinvocation);
                if (methodParameter != callbackParams[i].ParameterType)
                    throw new InvalidOperationException(argsDontMatch);
            }
        }
        private void AssertReturnTypeMatch(Delegate value)
        {
            if (GenericsUtil.GetRealType(method.ReturnType, Originalinvocation)
                .IsAssignableFrom(value.Method.ReturnType) == false)
                throw new InvalidOperationException("The delegate return value should be assignable from " + method.ReturnType.FullName);
        }
        #endregion
    }
}
