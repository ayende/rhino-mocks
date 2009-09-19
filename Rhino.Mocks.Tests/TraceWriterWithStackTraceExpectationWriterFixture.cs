namespace Rhino.Mocks.Tests
{
	using System.IO;
	using MbUnit.Framework;
	using Rhino.Mocks.Impl;

	[TestFixture]
	public class TraceWriterWithStackTraceExpectationWriterFixture
	{
		[Test]
		public void WillPrintLogInfoWithStackTrace()
		{
			TraceWriterWithStackTraceExpectationWriter expectationWriter = new TraceWriterWithStackTraceExpectationWriter();
			StringWriter writer = new StringWriter();
			expectationWriter.AlternativeWriter = writer;

			RhinoMocks.Logger = expectationWriter;

			MockRepository mocks = new MockRepository();
			IDemo mock = mocks.StrictMock<IDemo>();
			mock.VoidNoArgs();
			mocks.ReplayAll();
			mock.VoidNoArgs();
			mocks.VerifyAll();

			Assert.Contains(writer.GetStringBuilder().ToString(),
				"WillPrintLogInfoWithStackTrace");
		}
	}
}