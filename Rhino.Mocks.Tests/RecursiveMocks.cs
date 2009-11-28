#if DOTNET35
using System;
using System.Collections;
using System.Linq;
using Xunit;
using Rhino.Mocks.Impl;

namespace Rhino.Mocks.Tests
{
    
    public class RecursiveMocks
    {
        [Fact]
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

            Assert.Equal("ayende", customer.Name);
            Assert.Equal(1, customer.Id);
        }

        [Fact]
        public void CanUseRecursiveMocksSimpler()
        {
            var mockService = MockRepository.GenerateMock<IMyService>();

            mockService.Expect(x => x.Identity.Name).Return("foo");

            Assert.Equal("foo", mockService.Identity.Name);
        }

		[Fact(Skip = "Not supported right now as per Oren")]
        public void CanUseRecursiveMocksSimplerAlternateSyntax()
        {
            var mockService = MockRepository.GenerateMock<IMyService>();

            Expect.Call(mockService.Identity.Name).Return("foo");

            Assert.Equal("foo", mockService.Identity.Name);
        }

		[Fact(Skip = "Not supported in replay mode")]
        public void WillGetSameInstanceOfRecursedMockForGenerateMockStatic()
        {
            var mock = MockRepository.GenerateMock<IMyService>();

            IIdentity i1 = mock.Identity;
            IIdentity i2 = mock.Identity;

            Assert.Same(i1, i2);
            Assert.NotNull(i1);
        }

		[Fact(Skip = "Not supported in replay mode")]
        public void WillGetSameInstanceOfRecursedMockInReplayMode()
        {
            RhinoMocks.Logger = new TraceWriterExpectationLogger(true, true, true);

            MockRepository mocks = new MockRepository();
            var mock = mocks.DynamicMock<IMyService>();
            mocks.Replay(mock);

            IIdentity i1 = mock.Identity;
            IIdentity i2 = mock.Identity;

            Assert.Same(i1, i2);
            Assert.NotNull(i1);
        }

        [Fact]
        public void WillGetSameInstanceOfRecursedMockWhenNotInReplayMode()
        {
            RhinoMocks.Logger = new TraceWriterExpectationLogger(true,true,true);

            var mock = new MockRepository().DynamicMock<IMyService>();

            IIdentity i1 = mock.Identity;
            IIdentity i2 = mock.Identity;

            Assert.Same(i1, i2);
            Assert.NotNull(i1);
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
