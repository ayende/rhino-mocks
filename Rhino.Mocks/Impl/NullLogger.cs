using Castle.Core.Interceptor;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Doesn't log anything, just makes happy noises
	/// </summary>
	class NullLogger : IExpectationLogger
	{
		public void LogRecordedExpectation(IInvocation invocation, IExpectation expectation)
		{
		}

		public void LogReplayedExpectation(IInvocation invocation, IExpectation expectation)
		{
		}

		public void LogUnexpectedMethodCall(IInvocation invocation, string message)
		{
		}
	}
}