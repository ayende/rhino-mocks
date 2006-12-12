using System;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.MethodRecorders;
using Rhino.Mocks.Tests.MethodRecorders;

namespace Rhino.Mocks.Tests
{
	[TestFixture]	
	public class ComplexOredering
	{
		private IMethodRecorder recorder,nestedRecorder;
		private object proxy;
		private MethodInfo method;
		private IExpectation expectation;
		private object[] args;

		[SetUp]
		public void SetUp()
		{
			recorder = new UnorderedMethodRecorder();
			nestedRecorder = new UnorderedMethodRecorder();
			recorder.AddRecorder(nestedRecorder);

			proxy = new object();
			method = typeof (object).GetMethod("ToString");
			expectation = new AnyArgsExpectation(method);
			args = new object[0];
		}

		[Test]
		public void ComplexOrdering()
		{
			string expected = "Unordered: { Unordered: { Object.ToString(); } }";
			recorder.Record(proxy,method, expectation);
			Assert.AreEqual(expected, recorder.GetExpectedCallsMessage());
		}
	}
}
