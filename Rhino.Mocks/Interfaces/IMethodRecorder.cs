using System.Reflection;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Generated;

namespace Rhino.Mocks.Interfaces
{
	/// <summary>
	/// Records the actions on all the mocks created by a repository.
	/// </summary>
	public interface IMethodRecorder
	{
		/// <summary>
		/// Records the specified call with the specified args on the mocked object.
		/// </summary>
		void Record(object proxy, MethodInfo method, IExpectation expectation);

		/// <summary>
		/// Get the expectation for this method on this object with this arguments 
		/// </summary>
		IExpectation GetRecordedExpectation(object proxy, MethodInfo method, object[] args);

		/// <summary>
		/// This check the methods that were setup using the SetupResult.For()
		/// or LastCall.Repeat.Any() and that bypass the whole expectation model.
		/// </summary>
		IExpectation GetRepeatableExpectation(object proxy, MethodInfo method, object[] args);

		/// <summary>
		/// Gets the all expectations for a mocked object and method combination,
		/// regardless of the expected arguments / callbacks / contraints.
		/// </summary>
		/// <param name="proxy">Mocked object.</param>
		/// <param name="method">Method.</param>
		/// <returns>List of all relevant expectation</returns>
		ExpectationsList GetAllExpectationsForProxyAndMethod(object proxy, MethodInfo method);

		/// <summary>
		/// Gets the all expectations for proxy.
		/// </summary>
		/// <param name="proxy">Mocked object.</param>
		/// <returns>List of all relevant expectation</returns>
		ExpectationsList GetAllExpectationsForProxy(object proxy);

        /// <summary>
        /// Removes all the repeatable expectations for proxy.
        /// </summary>
        /// <param name="proxy">Mocked object.</param>
        void RemoveAllRepeatableExpectationsForProxy(object proxy);

		/// <summary>
		/// Replaces the old expectation with the new expectation for the specified proxy/method pair.
		/// This replace ALL expectations that equal to old expectations.
		/// </summary>
		/// <param name="proxy">Proxy.</param>
		/// <param name="method">Method.</param>
		/// <param name="oldExpectation">Old expectation.</param>
		/// <param name="newExpectation">New expectation.</param>
		void ReplaceExpectation(object proxy, MethodInfo method, IExpectation oldExpectation, IExpectation newExpectation);

		/// <summary>
		/// Gets a value indicating whether this instance has expectations that weren't satisfied yet.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has expectations; otherwise, <c>false</c>.
		/// </value>
		bool HasExpectations { get; }

		/// <summary>
		/// Adds the recorder and turn it into the active recorder.
		/// </summary>
		/// <param name="recorder">Recorder.</param>
		void AddRecorder(IMethodRecorder recorder);

		/// <summary>
		/// Moves to previous recorder.
		/// </summary>
		bool MoveToPreviousRecorder();

		/// <summary>
		/// Gets the recorded expectation or null.
		/// </summary>
		IExpectation GetRecordedExpectationOrNull(object proxy, MethodInfo method, object[] args);

		/// <summary>
		/// Gets the next expected calls string.
		/// </summary>
		string GetExpectedCallsMessage();

		/// <summary>
		/// Moves to parent recorder.
		/// </summary>
		void MoveToParentReplayer();

		/// <summary>
		/// Set the expectation so it can repeat any number of times.
		/// </summary>
		void AddToRepeatableMethods(object proxy, MethodInfo method, IExpectation expectation);

		/// <summary>
		/// Removes the expectation from the recorder
		/// </summary>
		void RemoveExpectation(IExpectation expectation);

		/// <summary>
		/// Clear the replayer to call (and all its chain of replayers)
		/// This also removes it from the list of expectations, so it will never be considered again
		/// </summary>
		void ClearReplayerToCall(IMethodRecorder childReplayer);

		/// <summary>
		/// Get the expectation for this method on this object with this arguments 
		/// </summary>
		ExpectationViolationException UnexpectedMethodCall(object proxy, MethodInfo method, object[] args);
    }
}