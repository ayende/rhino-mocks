using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using System.ComponentModel;

namespace RhinoMocksWithSyntax
{
    /// <summary>
    /// An alternative way of specifying expectations for a block of code
    /// to be verified uses the With class.
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+With-Syntax.ashx"/>
    [TestFixture]
    public class RhinoMocksWithSyntaxTest
    {
        [Test]
        public void withSyntaxeTest()
        {
            // Prepare mock repository
            MockRepository mocks = new MockRepository();
            IDependency dependency = mocks.StrictMock<IDependency>();

            object result = null;

            With.Mocks(mocks).Expecting(delegate
            {
                // Record expectations
                Expect.Call(dependency.SomeMethod()).Return(null);
            })
            .Verify(delegate
            {
                // Replay and validate interaction
                ComponentImplementation underTest = new ComponentImplementation(dependency);
                result = underTest.TestMethod();
            });

            // Post-interaction assertions
            Assert.IsNull(result);
        }

        [Test]
        public void withSyntaxeTest2()
        {
            // Prepare mock repository
            MockRepository mocks = new MockRepository();
            IDependency dependency = mocks.StrictMock<IDependency>();
            IAnotherDependency anotherDependency = mocks.StrictMock<IAnotherDependency>();

            object result = null;

            With.Mocks(mocks).ExpectingInSameOrder(delegate
            {
                // Record expectations which must be met in the exact same order
                Expect.Call(dependency.SomeMethod()).Return(null);
                anotherDependency.SomeOtherMethod();
            })
            .Verify(delegate
            {
                // Replay and validate interaction
                ComponentImplementation underTest = new ComponentImplementation(dependency, anotherDependency);
                result = underTest.TestMethod();
            });

            // Post-interaction assertions
            Assert.IsNull(result);
        }
    }
}
