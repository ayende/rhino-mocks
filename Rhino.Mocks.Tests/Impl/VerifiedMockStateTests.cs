using System;
using NUnit.Framework;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.Impl
{
	[TestFixture]
	public class VerifiedMockStateTests
	{
		IMockState verify;

		[SetUp]
		public void Setup()
		{
			verify = new VerifiedMockState(null);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "This action is invalid when the mock object is in verified state.")]
		public void ThrowsOnLastMethodOptions()
		{
			verify.LastMethodOptions.Return(null);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "This action is invalid when the mock object is in verified state.")]
		public void ThrowOnMethodCall()
		{
			verify.MethodCall(null, typeof(Object).GetMethod("ToString"));
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "This action is invalid when the mock object is in verified state.")]
		public void ThrowsOnVerify()
		{
			verify.Verify();
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "This action is invalid when the mock object is in verified state.")]
		public void ThrowsOnVerifyState()
		{
			verify = verify.VerifyState;
		}


		[Test]
		[ExpectedException(typeof (InvalidOperationException), "This action is invalid when the mock object is in verified state.")]
		public void ThrowsOnReplay()
		{
			verify.Replay();
		}
	}
}
