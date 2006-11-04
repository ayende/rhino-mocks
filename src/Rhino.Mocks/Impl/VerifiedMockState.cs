using System;
using System.Reflection;
using Rhino.Mocks.Interfaces;
using Castle.DynamicProxy;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Throw an object already verified when accessed
	/// </summary>
	public class VerifiedMockState : IMockState
	{
        IMockState previous;

        /// <summary>
        /// Create a new instance of VerifiedMockState 
        /// </summary>
        /// <param name="previous">The previous mock state, used to get the initial record state</param>
        public VerifiedMockState(IMockState previous)
        {
            this.previous = previous;
        }

		/// <summary>
		/// Gets the matching verify state for this state
		/// </summary>
		public IMockState VerifyState
		{
			get { throw InvalidInVerifiedState(); }
		}

		/// <summary>
		/// Add a method call for this state' mock.
		/// </summary>
        /// <param name="invocation">The invocation for this method</param>
		/// <param name="method">The method that was called</param>
		/// <param name="args">The arguments this method was called with</param>
        public object MethodCall(IInvocation invocation, MethodInfo method, params object[] args)
		{
			if(method.Name == "Finalize") // skip finalizers
				return null;
			throw InvalidInVerifiedState();
		}

		/// <summary>
		/// Verify that this mock expectations have passed.
		/// </summary>
		public void Verify()
		{
			throw InvalidInVerifiedState();
		}

		/// <summary>
		/// Verify that we can move to replay state and move 
		/// to the reply state.
		/// </summary>
		public IMockState Replay()
		{
			throw InvalidInVerifiedState();
		}

        /// <summary>
        /// Gets a mock state that match the original mock state of the object.
        /// </summary>
        public virtual IMockState BackToRecord()
        {
            return previous.BackToRecord();
        }

		/// <summary>
		/// Get the options for the last method call
		/// </summary>
		public IMethodOptions LastMethodOptions
		{
			get { throw InvalidInVerifiedState(); }
		}

		/// <summary>
		/// Set the exception to throw when Verify is called.
		/// This is used to report exception that may have happened but where caught in the code.
		/// This way, they are reported anyway when Verify() is called.
		/// </summary>
		public void SetExceptionToThrowOnVerify(Exception ex)
		{
			//not implementing this, we are already verified.
		}

		private Exception InvalidInVerifiedState()
		{
			return new InvalidOperationException("This action is invalid when the mock object is in verified state.");
		}
	}
}
