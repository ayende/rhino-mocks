using NUnit.Framework;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Generated;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.MethodRecorders;

namespace Rhino.Mocks.Tests.MethodRecorders
{
	[TestFixture]
	public class UnorderedMethodRecorderTests : IMethodRecorderTests
	{
		[Test]
		public void CanRecordMethodsAndVerifyThem()
		{
			UnorderedMethodRecorder recorder = new UnorderedMethodRecorder();
			recorder.Record(demo, voidNoArgs, new AnyArgsExpectation(voidNoArgs));
			recorder.Record(demo, voidThreeArgs, new AnyArgsExpectation(voidNoArgs));

			Assert.IsNotNull(recorder.GetRecordedExpectation(demo, voidThreeArgs, new object[0]));
			Assert.IsNotNull(recorder.GetRecordedExpectation(demo, voidNoArgs, new object[0]));
		}


		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "IDemo.VoidNoArgs(); Expected #1, Actual #2.")]
		public void ReplayUnrecordedMethods()
		{
			UnorderedMethodRecorder recorder = new UnorderedMethodRecorder();
			recorder.Record(demo, voidNoArgs, new AnyArgsExpectation(voidNoArgs));
			recorder.Record(demo, voidThreeArgs, new AnyArgsExpectation(voidNoArgs));

			Assert.IsNotNull(recorder.GetRecordedExpectation(demo, voidThreeArgs, new object[0]));
			Assert.IsNotNull(recorder.GetRecordedExpectation(demo, voidNoArgs, new object[0]));

			recorder.GetRecordedExpectation(demo, voidNoArgs, new object[0]);
		}

		protected override IMethodRecorder CreateRecorder()
		{
			return new UnorderedMethodRecorder();
		}
	}
}