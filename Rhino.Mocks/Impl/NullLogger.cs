using Castle.Core.Interceptor;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Doesn't log anything, just makes happy noises
	/// </summary>
	public class NullLogger : IExpectationLogger
	{
		/// <summary>
		/// Logs the expectation as is was recorded
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="expectation">The expectation.</param>
		public void LogRecordedExpectation(IInvocation invocation, IExpectation expectation)
		{
		}

		/// <summary>
		/// Logs the expectation as it was recorded
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="expectation">The expectation.</param>
		public void LogReplayedExpectation(IInvocation invocation, IExpectation expectation)
		{
		}

		/// <summary>
		/// Logs the unexpected method call.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="message">The message.</param>
		public void LogUnexpectedMethodCall(IInvocation invocation, string message)
		{
		}
	}
}