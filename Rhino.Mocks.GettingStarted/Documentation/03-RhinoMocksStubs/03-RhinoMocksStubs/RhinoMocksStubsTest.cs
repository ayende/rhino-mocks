using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace RhinoMocksStubs
{
    /// <summary>
    /// You can use mocks as stubs, 
    /// you can create a dynamic mock and call PropertyBehavior on its methods.
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+-+Stubs.ashx"/>
    [TestFixture]
    public class RhinoMocksStubsTest
    {
        /// <summary>
        /// Wrong way to create a stub, it's not a true test
        /// </summary>
        [Test]
        public void CreateAnimalStub()
        {
            MockRepository mocks = new MockRepository();
            IAnimal animal = mocks.DynamicMock<IAnimal>();
            Expect.Call(animal.Legs).PropertyBehavior();
            Expect.Call(animal.Eyes).PropertyBehavior();
            Expect.Call(animal.Name).PropertyBehavior();
            Expect.Call(animal.Species).PropertyBehavior();
        }

        /// <summary>
        /// Wrong way to create a stub with AAA method, it's not a true test
        /// </summary>
        [Test]
        public void CreateAnimalStub_AAA()
        {
            //Arrange
            MockRepository mocks = new MockRepository();
            IAnimal animal = MockRepository.GenerateMock<IAnimal>();
            animal.Expect(a => a.Legs).PropertyBehavior();
            animal.Expect(a => a.Eyes).PropertyBehavior();
            animal.Expect(a => a.Name).PropertyBehavior();
            animal.Expect(a => a.Species).PropertyBehavior();
        }

        /// <summary>
        /// Good way to create a stub using object implementation in place of static one
        /// </summary>
        [Test]
        public void CreateAnimalStub_GenerateStub()
        {

            MockRepository mocks = new MockRepository();
            IAnimal animal = mocks.Stub<IAnimal>();

            animal.Name = "Snoopy";

            Assert.AreEqual("Snoopy", animal.Name);
        }

        /// <summary>
        /// Good way to create a stub with AAA methods
        /// </summary>
        [Test]
        public void CreateAnimalStub_GenerateStub_AAA()
        {
            //Arrange
            IAnimal animal = MockRepository.GenerateStub<IAnimal>();

            //Act
            animal.Name = "Snoopy";

            //Assert
            Assert.AreEqual("Snoopy", animal.Name);
        }
    }
}
