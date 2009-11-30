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
        /// Wrong way to create a stub
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
        /// Good way to create a stub
        /// </summary>
        [Test]
        // Tests stub creation with the GenerateStub method
        public void CreateAnimalStub_GenerateStub()
        {
            IAnimal animal = MockRepository.GenerateStub<IAnimal>();
            animal.Name = "Snoopy";
            Assert.AreEqual("Snoopy", animal.Name);
        }

        /// <summary>
        /// Second way by using object implementation in place of static one
        /// </summary>
        [Test]
        public void CreateAnimalStub_MockRepositoryStub()
        {

            MockRepository mocks = new MockRepository();
            IAnimal animal = mocks.Stub<IAnimal>();

            animal.Name = "Snoopy";

            Assert.AreEqual("Snoopy", animal.Name);
        }
    }
}
