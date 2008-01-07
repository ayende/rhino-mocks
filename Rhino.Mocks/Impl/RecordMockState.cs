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
using System.Reflection;
using Castle.Core.Interceptor;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Utilities;
using Castle.DynamicProxy;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Records all the expectations for a mock
	/// </summary>
	public class RecordMockState : IMockState
	{
		#region Variables

		private MockRepository repository;
		private readonly IMockedObject mockedObject;
		private int methodCallsCount = 0;
		private IExpectation lastExpectation;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the last expectation.
		/// </summary>
		public IExpectation LastExpectation
		{
			get { return lastExpectation; }
			set { lastExpectation = value; }
		}

		/// <summary>
		/// Gets the total method calls count.
		/// </summary>
		public int MethodCallsCount
		{
			get { return methodCallsCount; }
		}

		/// <summary>
		/// Get the options for the last method call
		/// </summary>
		public IMethodOptions<T> GetLastMethodOptions<T>()
		{
			if (lastExpectation == null)
				throw new InvalidOperationException("There is no matching last call on this object. Are you sure that the last call was a virtual or interface method call?");
			return new MethodOptions<T>(repository, this, mockedObject, lastExpectation);
		}

		/// <summary>
		/// Get the options for the last method call
		/// </summary>
		public IMethodOptions<object> LastMethodOptions
		{
			get { return GetLastMethodOptions<object>(); }
		}

		/// <summary>
		/// Set the exception to throw when Verify is called.
		/// This is used to report exception that may have happened but where caught in the code.
		/// This way, they are reported anyway when Verify() is called.
		/// </summary>
		public void SetExceptionToThrowOnVerify(Exception ex)
		{
			//not implementing this, since there is never a call to Verify() anyway.
		}

		/// <summary>
		/// Gets the matching verify state for this state
		/// </summary>
		public IMockState VerifyState
		{
			get { throw InvalidOperationOnRecord(); }
		}

		#endregion

		#region C'tor

		/// <summary>
		/// Creates a new <see cref="RecordMockState"/> instance.
		/// </summary>
		/// <param name="repository">Repository.</param>
		/// <param name="mockedObject">The proxy that generates the method calls</param>
		public RecordMockState(IMockedObject mockedObject, MockRepository repository)
		{
			Validate.IsNotNull(mockedObject, "proxy");
			Validate.IsNotNull(repository, "repository");
			this.repository = repository;
			this.mockedObject = mockedObject;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add a method call for this state' mock.
		/// </summary>
        /// <param name="invocation">The invocation for this method</param>
		/// <param name="method">The method that was called</param>
		/// <param name="args">The arguments this method was called with</param>
		public object MethodCall(IInvocation invocation, MethodInfo method, params object[] args)
		{
			AssertPreviousMethodIsClose();
			repository.lastMockedObject = mockedObject;
			MockRepository.lastRepository = repository;
		    IExpectation expectation = BuildDefaultExpectation(invocation, method, args);
		    repository.Recorder.Record(mockedObject, method, expectation);
			lastExpectation = expectation;
			methodCallsCount++;
			RhinoMocks.Logger.LogRecordedExpectation(invocation, expectation);
			return ReturnValueUtil.DefaultValue(method.ReturnType, invocation);
		}

	    /// <summary>
		/// Verify that we can move to replay state and move 
		/// to the reply state.
		/// </summary>
		public virtual IMockState Replay()
		{
			AssertPreviousMethodIsClose();
			return DoReplay();
		}

		/// <summary>
		/// Verify that we can move to replay state and move 
		/// to the reply state.
		/// </summary>
		protected virtual IMockState DoReplay()
		{
			return new ReplayMockState(this);
		}

		/// <summary>
		/// Verify that this mock expectations have passed.
		/// </summary>
		public virtual void Verify()
		{
			throw InvalidOperationOnRecord();
		}

        /// <summary>
        /// Gets a mock state that match the original mock state of the object.
        /// </summary>
        public virtual IMockState BackToRecord()
        {
            return new RecordMockState(mockedObject, repository);
        }

		#endregion

		#region Private Methods

		/// <summary>
		/// Asserts the previous method is closed (had an expectation set on it so we can replay it correctly)
		/// </summary>
		protected virtual void AssertPreviousMethodIsClose()
		{
			if (lastExpectation != null && !lastExpectation.ActionsSatisfied)
				throw new InvalidOperationException("Previous method '" + lastExpectation.ErrorMessage + "' requires a return value or an exception to throw.");
		}

		private Exception InvalidOperationOnRecord()
		{
			return new InvalidOperationException("This action is invalid when the mock object is in record state.");
        }

        private static IExpectation BuildDefaultExpectation(IInvocation invocation, MethodInfo method, object[] args)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (!Array.Exists(parameters, delegate(ParameterInfo p) { return p.IsOut; }))
            {
                return new ArgsEqualExpectation(invocation, args);
            }

            //The value of an incoming out parameter variable is ignored
            AbstractConstraint[] constraints = new AbstractConstraint[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                constraints[i] = parameters[i].IsOut ? Is.Anything() : Is.Equal(args[i]);
            }
            return new ConstraintsExpectation(invocation, constraints);
        }

        #endregion

        #region Internal
        internal MockRepository Repository
        {
            get { return repository; }
        }

        internal IMockedObject Proxy
        {
            get { return mockedObject; }
        }

        #endregion
	}
}
