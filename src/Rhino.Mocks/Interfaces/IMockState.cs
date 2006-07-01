using System.Reflection;
using Castle.DynamicProxy;

namespace Rhino.Mocks.Interfaces
{
	/// <summary>
	/// Different actions on this mock
	/// </summary>
	public interface IMockState
	{
		/// <summary>
		/// Add a method call for this state' mock.
		/// </summary>
        /// <param name="invocation">The invocation for this method</param>
		/// <param name="method">The method that was called</param>
		/// <param name="args">The arguments this method was called with</param>
        object MethodCall(IInvocation invocation, MethodInfo method, params object[] args);

		/// <summary>
		/// Verify that this mock expectations have passed.
		/// </summary>
		void Verify();

		/// <summary>
		/// Gets the matching verify state for this state
		/// </summary>
		IMockState VerifyState { get; }

		/// <summary>
		/// Verify that we can move to replay state and move 
		/// to the reply state.
		/// </summary>
		IMockState Replay();

        /// <summary>
        /// Gets a mock state that match the original mock state of the object.
        /// </summary>
        IMockState BackToRecord();

		/// <summary>
		/// Get the options for the last method call
		/// </summary>
		IMethodOptions LastMethodOptions { get; }
	}
}