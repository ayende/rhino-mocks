using NUnit.Framework;

namespace Rhino.Mocks.GettingStarted
{
    [TestFixture]
    public class Stubs
    {
        [Test]
        public void Demonstrate_Stub_Implements_the_passed_type()
        {
            // Arrange
            
            // Act
            var stub = MockRepository.GenerateStub<IFoo>();

            // Assert
            Assert.That(stub.Implements<IFoo>());
        }

        /// <summary>
        /// When you need to mock a read-only property of a class.
        /// </summary>
        [Test]
        public void How_to_Stub_out_your_own_value_of_a_ReadOnlyProperty()
        {
            // Arrange
            var foo = MockRepository.GenerateStub<IFoo>();
            foo.Stub(x => x.ID).Return(123);

            // Act
            var id = foo.ID;

            // Assert
            Assert.That(id, Is.EqualTo(123));
        }

        /// <summary>
        /// This is just a sample interface, what it is or does isn't really relevant. It could
        /// be IUser of IOrder
        /// </summary>
        public interface IFoo
        {
            int ID { get; }
            string Name { get; set; }
        }

        public class Foo : IFoo
        {
            private int id;

            public int ID
            {
                get { return id; }
            }

            public string Name { get; set; }
        }
    }
}