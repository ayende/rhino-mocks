#if DOTNET35
using System;
using System.Collections;
using System.Linq;
using MbUnit.Framework;
using Rhino.Mocks.Impl;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class RecursiveMocks
    {
        [Test]
        public void CanUseRecursiveMocks()
        {
            var session = MockRepository.GenerateMock<ISession>();
            session.Stub(x =>
                         x.CreateCriteria(typeof (Customer))
                             .List()
                ).Return(new[] {new Customer {Id = 1, Name = "ayende"}});

            Customer customer = session.CreateCriteria(typeof (Customer))
                .List()
                .Cast<Customer>()
                .First();

            Assert.AreEqual("ayende", customer.Name);
            Assert.AreEqual(1, customer.Id);
        }

        [Test]
        public void CanUseRecursiveMocksSimpler()
        {
            var mockService = MockRepository.GenerateMock<IMyService>();

            mockService.Expect(x => x.Identity.Name).Return("foo");

            Assert.AreEqual("foo", mockService.Identity.Name);
        }

        [Test]
        [Ignore("Not supported right now as per Oren")]
        public void CanUseRecursiveMocksSimplerAlternateSyntax()
        {
            var mockService = MockRepository.GenerateMock<IMyService>();

            Expect.Call(mockService.Identity.Name).Return("foo");

            Assert.AreEqual("foo", mockService.Identity.Name);
        }

        [Test]
        [Ignore("Not supported in replay mode")]
        public void WillGetSameInstanceOfRecursedMockForGenerateMockStatic()
        {
            var mock = MockRepository.GenerateMock<IMyService>();

            IIdentity i1 = mock.Identity;
            IIdentity i2 = mock.Identity;

            Assert.AreSame(i1, i2);
            Assert.IsNotNull(i1);
        }

        [Test]
        [Ignore("Not supported in replay mode")]
        public void WillGetSameInstanceOfRecursedMockInReplayMode()
        {
            RhinoMocks.Logger = new TraceWriterExpectationLogger(true, true, true);

            MockRepository mocks = new MockRepository();
            var mock = mocks.DynamicMock<IMyService>();
            mocks.Replay(mock);

            IIdentity i1 = mock.Identity;
            IIdentity i2 = mock.Identity;

            Assert.AreSame(i1, i2);
            Assert.IsNotNull(i1);
        }

        [Test]
        public void WillGetSameInstanceOfRecursedMockWhenNotInReplayMode()
        {
            RhinoMocks.Logger = new TraceWriterExpectationLogger(true,true,true);

            var mock = new MockRepository().DynamicMock<IMyService>();

            IIdentity i1 = mock.Identity;
            IIdentity i2 = mock.Identity;

            Assert.AreSame(i1, i2);
            Assert.IsNotNull(i1);
        }

        public interface ISession
        {
            ICriteria CreateCriteria(Type type);
        }

        public interface ICriteria
        {
            IList List();
        }
        public class Customer
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        public interface IIdentity
        {
            string Name { get; set; }
        }

        public interface IMyService
        {
            IIdentity Identity { get; set; }
        }
    }
}
#endif
