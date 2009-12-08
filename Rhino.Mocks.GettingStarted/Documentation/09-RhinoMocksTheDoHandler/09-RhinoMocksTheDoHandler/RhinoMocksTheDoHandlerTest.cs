using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace RhinoMocksTheDoHandler
{

    /// <summary>
    /// There are times when the returning a static value is not good enough for the scenario
    /// that you are testing, so for those cases, you can use the Do() handler
    /// to add custom behavior when the method is called. 
    /// 
    /// In general, the Do() handler simply replaces the method call. 
    /// 
    /// Its return value will be returned from the mocked call (as well as any exception thrown).
    /// 
    /// The handler's signature must match the method signature, 
    /// since it gets the same parameters as the call.
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+The+Do()+Handler.ashx"/>
    /// <remarks>
    /// If you've a complex logic going on for the test, 
    /// you should consider making a class manually, 
    /// it will probably be easier in the long run.
    /// </remarks>
    [TestFixture]
    public class RhinoMocksTheDoHandlerTest
    {
        delegate string NameSourceDelegate(string first, string surname);

        private string Formal(string first, string surname)
        {
            return first + " " + surname;
        }

        /// <summary>
        /// A speaker object can introduce itself formally or informally, 
        /// so we'll separate the name creation into a separate class. 
        /// 
        /// We want to test the speaker in isolation and mock the name creation, 
        /// so we use this code (contrived again, I'm afraid)
        /// </summary>
        [Test]
        public void SayHelloWorld()
        {
            MockRepository mocks = new MockRepository();
            INameSource nameSource = mocks.StrictMock<INameSource>();

            Expect.Call(nameSource.CreateName(null, null)).IgnoreArguments().
                Do(new NameSourceDelegate(Formal));

            mocks.ReplayAll();

            string expected = "Hi, my name is Ayende Rahien";
            string actual = new Speaker("Ayende", "Rahien", nameSource).Introduce();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SayHelloWorld_AAA()
        {
            //Arrange
            INameSource nameSource = MockRepository.GenerateStrictMock<INameSource>();
            nameSource.Expect(n => n.CreateName(null, null)).IgnoreArguments().
                Do(new NameSourceDelegate(Formal)); 

            
            //Act
            string actual = new Speaker("Ayende", "Rahien", nameSource).Introduce();

            //Assert
            string expected = "Hi, my name is Ayende Rahien";
            Assert.AreEqual(expected, actual);
            nameSource.VerifyAllExpectations();
        }
    }
}
