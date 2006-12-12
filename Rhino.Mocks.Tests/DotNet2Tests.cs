#if dotNet2
using System;
using NUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class DotNet2Tests
	{
		MockRepository mocks;
        IDotNet2Features demo;
		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
            demo = mocks.DynamicMock<IDotNet2Features>();
		}

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void DefaultValueOfNullableIsNull()
        {
            mocks.ReplayAll();
            Assert.IsNull(demo.NullableInt(3));
        }

        [Test]
        public void CanUseNullAsReturnValueForNullables()
        {
            Expect.Call(demo.NullableInt(5)).Return(null);
            mocks.ReplayAll();
            Assert.IsNull(demo.NullableInt(5));
        }

        [Test]
        public void CanPassNonNullableValues()
        {
            Expect.Call(demo.NullableInt(53)).Return(5);
            mocks.ReplayAll();
            Assert.AreEqual(5, demo.NullableInt(53));
        }
    
        internal interface IDotNet2Features
        {
            int? NullableInt(int i);
        }
	}
}
#endif