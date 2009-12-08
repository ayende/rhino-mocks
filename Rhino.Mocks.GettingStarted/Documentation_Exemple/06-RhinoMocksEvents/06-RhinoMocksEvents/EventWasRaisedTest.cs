using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace RhinoMocksEvents
{
    /// <summary>
    /// Here is how you check that an event raises
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+Events.ashx"/>
    [TestFixture]
    public class EventWasRaisedTest
    {
        [Test]
        public void VerifyingThatAnEventWasFired()
        {
            MockRepository mocks = new MockRepository();
            IEventSubscriber subscriber = mocks.StrictMock<IEventSubscriber>();
            IWithEvents events = new WithEvents();
            // This doesn't create an expectation because no method is called on subscriber!! 
            events.Blah += new EventHandler(subscriber.Handler);
            subscriber.Handler(events, EventArgs.Empty);
            mocks.ReplayAll();
            events.RaiseEvent();
            mocks.VerifyAll();
        }
    }
}
