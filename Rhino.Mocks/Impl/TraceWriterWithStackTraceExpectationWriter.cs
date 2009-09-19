namespace Rhino.Mocks.Impl
{
	using System.Diagnostics;
	using System.IO;
	using Castle.Core.Interceptor;
	using Interfaces;
	using Utilities;

	/// <summary>
	/// Writes log information as stack traces about rhino mocks activity
	/// </summary>
	public class TraceWriterWithStackTraceExpectationWriter : IExpectationLogger
	{
		/// <summary>
		/// Allows to redirect output to a different location.
		/// </summary>
		public TextWriter AlternativeWriter;

		/// <summary>
		/// Logs the expectation as is was recorded
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="expectation">The expectation.</param>
		public void LogRecordedExpectation(IInvocation invocation, IExpectation expectation)
		{
			string methodCall = MethodCallUtil.StringPresentation(invocation, invocation.Method, invocation.Arguments);
			WriteLine("Recorded expectation: {0}", methodCall);
			WriteCurrentMethod();
		}

		private void WriteLine(string msg, params object[] args)
		{
			string result = string.Format(msg, args);
			if (AlternativeWriter != null)
			{
				AlternativeWriter.WriteLine(result);
				return;
			}
			Debug.WriteLine(result);
		}

		private void WriteCurrentMethod()
		{
			WriteLine(new StackTrace(true).ToString());
		}

		/// <summary>
		/// Logs the expectation as it was recorded
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="expectation">The expectation.</param>
		public void LogReplayedExpectation(IInvocation invocation, IExpectation expectation)
		{
			string methodCall = MethodCallUtil.StringPresentation(invocation, invocation.Method, invocation.Arguments);
			WriteLine("Replayed expectation: {0}", methodCall);
			WriteCurrentMethod();
		}

		/// <summary>
		/// Logs the unexpected method call.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="message">The message.</param>
		public void LogUnexpectedMethodCall(IInvocation invocation, string message)
		{
			string methodCall = MethodCallUtil.StringPresentation(invocation, invocation.Method, invocation.Arguments);
			WriteLine("{1}: {0}", methodCall, message);
			WriteCurrentMethod();
		}
	}
}