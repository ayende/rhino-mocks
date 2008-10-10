#if DOTNET35
using System;
using System.Collections;
using System.Linq;
using System.Security.AccessControl;
using MbUnit.Framework;

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
        [Ignore("the next step")]
        public void WillGetSameInstanceOfRecursedMock()
        {
            var mock = new MockRepository().DynamicMock<ISession>();
            Assert.AreSame(
                mock.CreateCriteria(typeof(Customer)),
                mock.CreateCriteria(typeof(Customer))
                );
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
    }
}
#endif