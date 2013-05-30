using System;
using Castle.Core.Interceptor;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Impl
{

    /// <summary>
    /// Write rhino mocks log info to the console
    /// </summary>
    public class ConsoleLogger : IExpectationLogger
    {
        #region IExpectationLogger Members

        /// <summary>
        /// Logs the message
        /// </summary>
        /// <param name="message">The message.</param>
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Logs the expectation as is was recorded
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="expectation">The expectation.</param>
        public void LogRecordedExpectation(IInvocation invocation, IExpectation expectation)
        {
            string methodCall = MethodCallUtil.StringPresentation(invocation, invocation.Method, invocation.Arguments);
            Console.WriteLine(string.Format("Recorded expectation: {0}", methodCall));
        }

        /// <summary>
        /// Logs the expectation as it was recorded
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="expectation">The expectation.</param>
        public void LogReplayedExpectation(IInvocation invocation, IExpectation expectation)
        {
            string methodCall = MethodCallUtil.StringPresentation(invocation, invocation.Method, invocation.Arguments);
            Console.WriteLine(string.Format("Replayed expectation: {0}", methodCall));
        }

        /// <summary>
        /// Logs the unexpected method call.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="message">The message.</param>
        public void LogUnexpectedMethodCall(IInvocation invocation, string message)
        {
            string methodCall = MethodCallUtil.StringPresentation(invocation, invocation.Method, invocation.Arguments);
            Console.WriteLine(string.Format("{1}: {0}", methodCall, message));
        }

        #endregion
    }
}