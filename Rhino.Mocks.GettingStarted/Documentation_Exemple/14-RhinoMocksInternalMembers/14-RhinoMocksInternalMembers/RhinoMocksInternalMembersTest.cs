using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using System.Runtime.CompilerServices;

//Then a class is mocked, a new class is generated
//at run-time which is derived from the mocked class.
//This generated class resides in a separate "temporary" assembly
//which is called "DynamicProxyGenAssembly2". So, 
//The InternalsVisibleTo attribute needs to be set on the target assembly 
//to allow access to its internal members from the temporary assembly; 
//otherwise, the mock object can't override the internal member
//as it doesn't have access to it (which is also why the mocked method must be marked as virtual).
//Note that this is true even if the unit test and the tested class are in the same assembly.
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

//If you are using strong-named assemblies, this can be achieved as such:
[assembly: InternalsVisibleTo(Rhino.Mocks.RhinoMocks.StrongName)]

namespace RhinoMocksInternalMembers
{
    /// <summary>
    /// You can also use Rhino Mocks to mock internal classes or methods
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+-+Internal+Methods.ashx"/>
    [TestFixture]
    public class RhinoMocksInternalMembersTest
    {
        [Test]
        public void MockingInternalMethodsAndPropertiesOfInternalClass()
        {
            Class1 testClass = new Class1();

            string testMethod = testClass.TestMethod();
            string testProperty = testClass.TestProperty;

            MockRepository mockRepository = new MockRepository();

            Class1 mockTestClass = mockRepository.StrictMock<Class1>();
            Expect.Call(mockTestClass.TestMethod()).Return("MockTestMethod");
            Expect.Call(mockTestClass.TestProperty).Return("MockTestProperty");

            mockRepository.ReplayAll();

            Assert.AreEqual("MockTestMethod", mockTestClass.TestMethod());
            Assert.AreEqual("MockTestProperty", mockTestClass.TestProperty);

            mockRepository.VerifyAll();
        }
    }
}
