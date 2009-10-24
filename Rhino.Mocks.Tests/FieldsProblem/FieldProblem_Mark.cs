using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	
	public class FieldProblem_Mark
	{
		[Fact]
		public void GoodExplanationForUsingRepeatNeverAndReturn()
		{
			MockRepository mocks = new MockRepository();
			ILogWriter eventLogMock = (ILogWriter)mocks.DynamicMock(typeof(ILogWriter));
			Log log = new Log(null, eventLogMock, "MOCK", true, false);

			Expect.Call(eventLogMock.WriteLog(EventLogEntryType.SuccessAudit, "MOCK", null, null, 0)).Return(true);
			
			Assert.Throws<InvalidOperationException>(
				"After specifying Repeat.Never(), you cannot specify a return value, exception to throw or an action to execute",
				() => Expect.Call(eventLogMock.WriteLog(EventLogEntryType.FailureAudit, "MOCK", null, null, 0))
				.Repeat.Never().Return(true));
		}

		//This is exactly like the one above, but the calls to repeat and return are reverse
		[Fact]
		public void GoodExplanationForUsingReturnAndRepeatNever()
		{
			MockRepository mocks = new MockRepository();
			ILogWriter eventLogMock = (ILogWriter)mocks.DynamicMock(typeof(ILogWriter));
			Log log = new Log(null, eventLogMock, "MOCK", true, false);

			Expect.Call(eventLogMock.WriteLog(EventLogEntryType.SuccessAudit, "MOCK", null, null, 0)).Return(true);
			Assert.Throws<InvalidOperationException>(
				"After specifying Repeat.Never(), you cannot specify a return value, exception to throw or an action to execute",
				() => Expect.Call(eventLogMock.WriteLog(EventLogEntryType.FailureAudit, "MOCK", null, null, 0))
				.Return(true).Repeat.Never());
		}
	}

	public class Log
	{
		private ILogWriter traceWriter;
		private ILogWriter eventLogWriter;
		private string systemLogging;
		private bool logSuccesAudit;
		private bool logFailureAudit;

		public Log(ILogWriter traceWriter, ILogWriter eventLogWriter, string system, bool logSuccesAudit, bool logFailureAudit)
		{
			this.traceWriter = traceWriter;
			this.eventLogWriter = eventLogWriter;
			systemLogging = system;
			this.logSuccesAudit = logSuccesAudit;
			this.logFailureAudit = logFailureAudit;
		}

		public void Audit(AuditOptions audit, string system, string component, string text, int eventId)
		{
			EventLogEntryType translatedEntryType;
			if (audit == AuditOptions.Succes)
				translatedEntryType = EventLogEntryType.SuccessAudit;
			else
				translatedEntryType = EventLogEntryType.FailureAudit;
			eventLogWriter.WriteLog(translatedEntryType, system, component, text, eventId);
		}
	}

	public enum AuditOptions
	{
		Succes,
		Failure
	}

	public interface ILogWriter
	{
		bool WriteLog(EventLogEntryType entryType, string system, string component, string text, int eventId);
	}
}
