using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MbUnit.Framework;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Tests.Expectations;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Tests
{
	/// <summary>
	/// Tests needed to get full coverage
	/// They won't test meaningful stuff neccesarily
	/// </summary>
	[TestFixture]
	public class Coverage
	{
			
		[Test]
		public void CanSerializeObjectNotMockObjectFromThisRepostiory()
		{
			ObjectNotMockFromThisRepositoryException e = new ObjectNotMockFromThisRepositoryException("ff");
			ObjectNotMockFromThisRepositoryException other = (ObjectNotMockFromThisRepositoryException)SerializeAndDeserialize(e);
			Assert.AreEqual("ff", other.Message );
		}

		[Test]
		public void CanTestConstaintForTruth()
		{
			if (Is.Null())
				return;
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),"The last method call EnumNoArgs was not an event add / remove method")]
		public void TryingToGetEventRaiserFromNonEvenTrhows()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.CreateMock<IDemo>();
			demo.EnumNoArgs();
			LastCall.GetEventRaiser();
		}

		[Test]
		public void UsingCallOriginalWithSetter()
		{
			MockRepository mocks = new MockRepository();
			WithParameters withParameters = mocks.CreateMock<WithParameters>(1);
			withParameters.Int = 5;
			LastCall.PropertyBehavior();
			mocks.ReplayAll();
			withParameters.Int = 12;
			withParameters.Int = 15;
			Assert.AreEqual(15, withParameters.Int);
			mocks.VerifyAll();
		}

		[Test]
		public void MockObjCanHandleGetEventSubscribersCallsWithoutEventsRegistered()
		{
			MockRepository mocks = new MockRepository();
			WithParameters withParameters = mocks.CreateMock<WithParameters>(1);
			IMockedObject mocked = (IMockedObject) withParameters;
			Delegate eventSubscribers = mocked.GetEventSubscribers("ff");
			Assert.IsNull(eventSubscribers);
		}

		[Test]
		public void MockObjEqualityCanHandleNonMockObjects()
		{
			int i = MockedObjectsEquality.Instance.GetHashCode("");
			Assert.AreNotEqual(0, i);
		}

		[Test]
		public void WillDisplayMethodUsingArraysCorrectly()
		{
			string result = MethodCallUtil.StringPresentation(null, GetType().GetMethod("MethodUsingArray"), new object[] { 1, new string[] { "a", "b" } });
			Assert.AreEqual("Coverage.MethodUsingArray(1, [\"a\", \"b\"]);",result );
		}

		public void MethodUsingArray(int i, string[] foo)
		{
		}

		private delegate string ToStringDelegate();

		[Test]
		[ExpectedException(typeof(InvalidOperationException),"Trying to run a Do() delegate when no arguments were matched to the expectation.")]
		public void TryingToPassNullToReturnOrThrowWithActionWillThrow()
		{
			AnyArgsExpectation expectation = new AnyArgsExpectation(new FakeInvocation(typeof(object).GetMethod("ToString")));
			expectation.ActionToExecute = (ToStringDelegate)delegate { return "fpp"; };
			expectation.ReturnOrThrow(null,null);
		}

		[Test]
		public void CanCallSetExceptionToThrowOnVerifyOnVerifiedMockState()
		{
			new VerifiedMockState(null).SetExceptionToThrowOnVerify(null);
		}

		[Test]
		public void WillGetPartialRecordFromPartialRecord()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.CreateMock<IDemo>();
			IMockState mockState = new RecordPartialMockState((IMockedObject)demo, mocks).BackToRecord();
			Assert.AreEqual(typeof(RecordPartialMockState), mockState.GetType());
		}

		[Test]
		public void WillGetPartialRecordFromPartialReplay()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.CreateMock<IDemo>();
			IMockState mockState = new ReplayPartialMockState(new RecordPartialMockState((IMockedObject)demo, mocks)).BackToRecord();
			Assert.AreEqual(typeof(RecordPartialMockState), mockState.GetType());
		}

		[Test]
		public void WillGetDynamicRecordFromDynamicReplay()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.CreateMock<IDemo>();
			IMockState mockState = new ReplayDynamicMockState(new RecordDynamicMockState((IMockedObject)demo, mocks)).BackToRecord();
			Assert.AreEqual(typeof(RecordDynamicMockState), mockState.GetType());
		}


		public static object SerializeAndDeserialize(object proxy)
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, proxy);
			stream.Position = 0;
			return formatter.Deserialize(stream);
		}

	}
}
