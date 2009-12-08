using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections;
using Rhino.Mocks;

namespace RhinoMocksMockingClasses
{
    /// <summary>
    /// Rhino Mocks supports mocking classes as well as interfaces. 
    /// In fact, it can even mock classes that don't have a default constructor!
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+Mocking+classes.ashx"/>
    /// <remarks>
    /// Note: A technical limitation with mocking classes
    /// is that the creation of a mock object invokes the target class constructor,
    /// due to the mock deriving from the target class.
    /// </remarks>
    [TestFixture]
    public class RhinoMocksMockingClassesTest
    {
        /// <summary>
        /// To create a stub on a class, use the GenerateStub() method for creating the mock. 
        /// </summary>
        /// <remarks>
        /// Use this ONLY if you do not wish to set expectations on your mock.
        /// You only want to use the mock as a STUB
        /// </remarks>
        [Test]
        public void AbuseArrayList_UsingGenerateStub()
        {
            ArrayList list = MockRepository.GenerateStub<ArrayList>();

            // Evaluate the values from the mock
            Assert.AreEqual(0, list.Capacity);
        }

        /// <summary>
        /// Use the generic StrictMock<>() if you are using .Net 2.0 or higher:
        /// </summary>
        [Test]
        public void AbuseArrayList_UsingCreateMockGenerics()
        {
            MockRepository mocks = new MockRepository();
            ArrayList list = mocks.StrictMock<ArrayList>();

            // Setup the expectation of a call on the mock
            Expect.Call(list.Capacity).Return(999);
            mocks.ReplayAll();

            // Evaluate the values from the mock
            Assert.AreEqual(999, list.Capacity);
            mocks.VerifyAll(); ;
        }
    }
}
