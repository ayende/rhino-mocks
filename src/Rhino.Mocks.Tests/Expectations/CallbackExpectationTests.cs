using System;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Tests.Callbacks;

namespace Rhino.Mocks.Tests.Expectations
{
	[TestFixture]
	public class CallbackExpectationTests :AbstractExpectationTests
	{
		private MockRepository mocks;
		private IDemo demo;
		private CallbackExpectation callback;
		private MethodInfo method;
		private bool callbackCalled;

		protected override IExpectation GetExpectation(MethodInfo m, Range r, int actual)
		{
			CallbackExpectation expectation = new CallbackExpectation(m, new DelegateDefinations.NoArgsDelegate(VoidNoArgs));
			SetupExpectation(expectation, r, actual);
			return expectation;
		}

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			demo = (IDemo) mocks.CreateMock(typeof (IDemo));
			method = typeof (IDemo).GetMethod("VoidThreeArgs");
			callbackCalled = false;
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),"Callback arguments didn't match the method arguments")]
		public void ExceptionWhenArgsDontMatch()
		{
			callback = new CallbackExpectation(method, new DelegateDefinations.NoArgsDelegate(VoidNoArgs));
		}

		[Test]
		public void CallMethodWhenTestIsExpected()
		{
			callback = new CallbackExpectation(method, new DelegateDefinations.ThreeArgsDelegate(ThreeArgsDelegateMethod));
			callback.IsExpected(new object[] {1, "", 3.3f});
			Assert.IsTrue(callbackCalled);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Callbacks must return a boolean")]
		public void CallbackDoesntReturnBool()
		{
			callback = new CallbackExpectation(method, new DelegateDefinations.VoidThreeArgsDelegate(VoidThreeArgsDelegateMethod));
			callback.IsExpected(new object[] {1, "", 3.3f});
			Assert.IsTrue(callbackCalled);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Callback arguments didn't match the method arguments")]
		public void CallbackWithDifferentSignature_NumArgsDifferent()
		{
			callback = new CallbackExpectation(method, new DelegateDefinations.StringDelegate("".StartsWith));
			callback.IsExpected(new object[] {1, "", 3.3f});
			Assert.IsTrue(callbackCalled);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "Callback arguments didn't match the method arguments")]
		public void CallBackWithDifferentSignature()
		{
			callback = new CallbackExpectation(method, new DelegateDefinations.IntArgDelegate(OneArg));
		}

		#region Implementation

		private bool VoidNoArgs()
		{
			return true;
		}

		private bool OneArg(int i)
		{
			return true;
		}

		private void VoidThreeArgsDelegateMethod(int i, string s, float f)
		{
		}

		private bool ThreeArgsDelegateMethod(int i, string s, float f)
		{
			callbackCalled = true;
			return true;
		}

		#endregion
	}
}