#if dotNet2
using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class MockingGenericInterfaces
    {
        MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void MockAGenericInterface()
        {
            IList<int> list = mocks.CreateMock<IList<int>>();
            Assert.IsNotNull(list);
            Expect.Call(list.Count).Return(5);
            mocks.ReplayAll();
            Assert.AreEqual(5, list.Count);
        }

        [Test]
        public void DynamicMockOfGeneric()
        {
            IList<int> list = mocks.DynamicMock<IList<int>>();
            Assert.IsNotNull(list);
            Expect.Call(list.Count).Return(5);
            mocks.ReplayAll();
            Assert.AreEqual(5, list.Count);
            list.Add(4);
        }
    }
}
#endif