using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using System.ComponentModel;
using NUnit.Framework;

namespace RhinoMocksRecordPlaybackSyntax
{
    /// <summary>
    /// Rhino Mocks has an alternative syntax to define and replay expectations. 
    /// The syntax makes heavy use of the using keyword in C#:
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+Record-playback+Syntax.ashx"/>
    [TestFixture]
    public class RhinoMocksRecordPlaybackSyntaxTest
    {
        [Test]
        public void RecordPlayBackSyntaxUsing()
        {
            // Prepare mock repository
            MockRepository mocks = new MockRepository();
            IDependency dependency = mocks.StrictMock<IDependency>();

            // Record expectations
            using (mocks.Record())
            {
                Expect
                    .Call(dependency.SomeMethod())
                    .Return(null);
            }

            // Replay and validate interaction
            // Replace mocks.ReplayAll && mocks.VerifyAll
            object result;
            using (mocks.Playback())
            {
                ComponentImplementation underTest = new ComponentImplementation(dependency);
                result = underTest.TestMethod();
            }

            // Post-interaction assertions
            Assert.IsNull(result);
        }
    }

}
