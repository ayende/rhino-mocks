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
    /// 
    /// </summary>
    public class TraceWriterExpectationLogger : IExpectationLogger
    {
        private bool _logRecorded = true;
        private bool _logReplayed = true;
        private bool _logUnexpected = true;

        /// <summary>
        /// 
        /// </summary>
        public TraceWriterExpectationLogger()
        {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logRecorded"></param>
        /// <param name="logReplayed"></param>
        /// <param name="logUnexpected"></param>
        public TraceWriterExpectationLogger(bool logRecorded, bool logReplayed, bool logUnexpected)
        {
            _logRecorded = logRecorded;
            _logReplayed = logReplayed;
            _logUnexpected = logUnexpected;
        }

        #region IExpectationLogger Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="expectation"></param>
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
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="expectation"></param>
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
        /// 
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="message"></param>
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
