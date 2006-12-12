using System;
using System.Reflection;
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
		int ActualCalls { get; }

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
		object ReturnOrThrow(object [] args);

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
	    object[] OutRefParams { set; }

		/// <summary>
		/// Documentation Message
		/// </summary>
		string Message { get; set; }
	}
}