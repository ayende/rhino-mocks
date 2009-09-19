using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Castle.Core.Interceptor;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Impl
{
    /// <summary>
    /// Write rhino mocks log info to the trace
    /// </summary>
    public class TraceWriterExpectationLogger : IExpectationLogger
    {
        private readonly bool _logRecorded = true;
        private readonly bool _logReplayed = true;
        private readonly bool _logUnexpected = true;

		/// <summary>
		/// Initializes a new instance of the <see cref="TraceWriterExpectationLogger"/> class.
		/// </summary>
        public TraceWriterExpectationLogger()
        {}

		/// <summary>
		/// Initializes a new instance of the <see cref="TraceWriterExpectationLogger"/> class.
		/// </summary>
		/// <param name="logRecorded">if set to <c>true</c> [log recorded].</param>
		/// <param name="logReplayed">if set to <c>true</c> [log replayed].</param>
		/// <param name="logUnexpected">if set to <c>true</c> [log unexpected].</param>
        public TraceWriterExpectationLogger(bool logRecorded, bool logReplayed, bool logUnexpected)
        {
            _logRecorded = logRecorded;
            _logReplayed = logReplayed;
            _logUnexpected = logUnexpected;
        }

        #region IExpectationLogger Members

		/// <summary>
		/// Logs the expectation as is was recorded
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="expectation">The expectation.</param>
        public void LogRecordedExpectation(IInvocation invocation, IExpectation expectation)
        {
            if (_logRecorded)
            {
                string methodCall =
                    MethodCallUtil.StringPresentation(invocation, invocation.Method, invocation.Arguments);
                Trace.WriteLine(string.Format("Recorded expectation: {0}", methodCall));
            }
        }

		/// <summary>
		/// Logs the expectation as it was recorded
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="expectation">The expectation.</param>
        public void LogReplayedExpectation(IInvocation invocation, IExpectation expectation)
        {
            if (_logReplayed)
            {
                string methodCall =
                    MethodCallUtil.StringPresentation(invocation, invocation.Method, invocation.Arguments);
                Trace.WriteLine(string.Format("Replayed expectation: {0}", methodCall));
            }
        }

		/// <summary>
		/// Logs the unexpected method call.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="message">The message.</param>
        public void LogUnexpectedMethodCall(IInvocation invocation, string message)
        {
            if (_logUnexpected)
            {
                string methodCall =
                    MethodCallUtil.StringPresentation(invocation, invocation.Method, invocation.Arguments);
                Trace.WriteLine(string.Format("{1}: {0}", methodCall, message));
            }
        }

        #endregion
    }
}
