using System;
using MbUnit.Framework;
using Rhino.Mocks.Impl;

namespace Rhino.Mocks.Tests.Impl
{
	[TestFixture]	
	public class MockedObjectsComparerTests
	{
		MockRepository mocks;
		IDemo one, two;

		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
			one = (IDemo)mocks.CreateMock(typeof(IDemo));
			two = (IDemo)mocks.CreateMock(typeof(IDemo));
		}

		[Test]
		public void FalseForDifferenceMocks()
		{
            bool condition = MockedObjectsEquality.Instance.Compare(one, two) == 0;
			Assert.IsFalse(condition);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException),"Both arguments to Compare() were not Mocked objects!")]
		public void ThrowForBothNonMocks()
		{
            bool condition = MockedObjectsEquality.Instance.Compare(new object(), new object()) == 0;
			Assert.IsFalse(condition);
		}


		[Test]
		public void TrueForSameObject()
		{
            bool condition = MockedObjectsEquality.Instance.Compare(one, one) == 0;
			Assert.IsTrue(condition);
		}

		[Test]
		public void FalseForOneMockAndOneNull()
		{
            bool condition = MockedObjectsEquality.Instance.Compare(one, null) == 0;
			Assert.IsFalse(condition);
            condition = MockedObjectsEquality.Instance.Compare(null, one) == 0;
			Assert.IsFalse(condition);
		}

		[Test]
		public void TrueForBothNulls()
		{
            bool condition = MockedObjectsEquality.Instance.Compare(null, null) == 0;
			Assert.IsTrue(condition);
		}

		[Test]
		public void FalseForOneMockAndOneNot()
		{
            bool condition = MockedObjectsEquality.Instance.Compare(one, new object()) == 0;
			Assert.IsFalse(condition);
            condition = MockedObjectsEquality.Instance.Compare(new object(), one) == 0;
			Assert.IsFalse(condition);
		}


	}
}
