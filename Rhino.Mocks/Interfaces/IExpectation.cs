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
using System.Reflection;
using Castle.DynamicProxy;
using Rhino.Mocks.Impl;

namespace Rhino.Mocks.Interfaces
{
	/// <summary>
	/// Interface to validate that a method call is correct.
	/// </summary>
	public interface IExpectation
	{
		/// <summary>
		/// Validate the arguments for the method.
		/// This method can be called numerous times, so be careful about side effects
		/// </summary>
		/// <param name="args">The arguments with which the method was called</param>
		bool IsExpected(object[] args);

		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value></value>
		string ErrorMessage { get; }

		/// <summary>
		/// Range of expected calls
		/// </summary>
		Range Expected { get; set; }

		/// <summary>
		/// Number of call actually made for this method
		/// </summary>
		int ActualCallsCount { get; }

		/// <summary>
		/// If this expectation is still waiting for calls.
		/// </summary>
		bool CanAcceptCalls { get; }

		/// <summary>
		/// Add an actual method call to this expectation
		/// </summary>
		void AddActualCall();

		/// <summary>
		/// The return value for a method matching this expectation
		/// </summary>
		object ReturnValue { get; set; }

		/// <summary>
		/// Gets or sets the exception to throw on a method matching this expectation.
		/// </summary>
		Exception ExceptionToThrow { get; set; }

		/// <summary>
		/// Returns the return value or throw the exception and setup any output / ref parameters
		/// that has been set.
		/// </summary>
		object ReturnOrThrow(IInvocation invocation, object [] args);

		/// <summary>
		/// Gets a value indicating whether this instance's action is staisfied.
		/// A staisfied instance means that there are no more requirements from
		/// this method. A method with non void return value must register either
		/// a return value or an exception to throw.
		/// </summary>
		bool ActionsSatisfied { get; }

		/// <summary>
		/// Gets the method this expectation is for.
		/// </summary>
		MethodInfo Method { get; }

		/// <summary>
		/// Gets or sets what special condtions there are for this method
		/// repeating.
		/// </summary>
		RepeatableOption RepeatableOption { get; set; }

		/// <summary>
		/// Gets a value indicating whether this expectation was satisfied
		/// </summary>
		bool ExpectationSatisfied { get; }

		/// <summary>
		/// Specify whatever this expectation has a return value set
		/// You can't check ReturnValue for this because a valid return value include null.
		/// </summary>
		bool HasReturnValue { get; }

        /// <summary>
        /// An action to execute when the method is matched.
        /// </summary>
		Delegate ActionToExecute { get; set; }

	    /// <summary>
	    /// Set the out / ref parameters for the method call.
	    /// The indexing is zero based and ignores any non out/ref parameter.
	    /// It is possible not to pass all the parameters. This method can be called only once.
	    /// </summary>
		object[] OutRefParams { get;  set; }

		/// <summary>
		/// Documentation Message
		/// </summary>
		string Message { get; set; }
		/// <summary>
		/// Gets the invocation for this expectation
		/// </summary>
		/// <value>The invocation.</value>
		IInvocation Originalinvocation { get;}

		/// <summary>
		/// Occurs when the exceptation is match on a method call
		/// </summary>
		event Action<MethodInvocation> WhenCalled;

		/// <summary>
		/// Builds the verification failure message.
		/// </summary>
		/// <returns></returns>
		string BuildVerificationFailureMessage();

        /// <summary>
        /// Allow to set the return value in the future, if it was already set.
        /// </summary>
        bool AllowTentativeReturn { set;  get; }
	}
}