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
        /// This is just a sample interface, what it is or does isn't really relevant. It could
        /// be IUser of IOrder
        /// </summary>
        public interface IFoo
        {
            string Name { get; set; }
        }
    }
}