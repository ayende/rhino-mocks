using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace RhinoMockDelegates
{
    /// <summary>
    /// Sometimes you want to be able to verify/affect the invocation of a delegate 
    /// (this is especially useful if you're used to functional programming) 
    /// and pass delegates around to do your bidding
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+Mocking+Delegates.ashx"/>
    [TestFixture]
    public class RhinoMocksMockingDelegatesTest
    {
        [Test]
        public void GenericDelegate()
        {
            MockRepository mocks = new MockRepository();
            Action<int> action = mocks.StrictMock<Action<int>>();
            for (int i = 0; i < 10; i++)
            {
                action(i);
            }
            mocks.ReplayAll();
            ForEachFromZeroToNine(action);
            mocks.VerifyAll();
        }

        private void ForEachFromZeroToNine(Action<int> act)
        {
            for (int i = 0; i < 10; i++)
            {
                act(i);
            }
        }
    }
}
