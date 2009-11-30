using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace RhinoMocksEvents
{
    /// <summary>
    /// Here is how you check that the object under test subscribes to an event on a mock object
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+Events.ashx"/>
    [TestFixture]
    public class SubscribeToEventTest
    {
        [Test]
        public void VerifyingThatEventWasAttached()
        {
            MockRepository mocks = new MockRepository();
            IWithEvents events = mocks.StrictMock<IWithEvents>();
            With.Mocks(mocks).Expecting(delegate
            {
                events.Blah +=new EventHandler(events_Blah);
            })
           .Verify(delegate
           {
               MethodThatSubscribeToEventBlah(events);
           });
        }

        public void MethodThatSubscribeToEventBlah(IWithEvents events)
        {
            events.Blah += new EventHandler(events_Blah);
        }

        void events_Blah(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        [Test]
        public void VerifyingThatEventWasAttached_AAA()
        {
            var events = MockRepository.GenerateMock<IWithEvents>();
            MethodThatSubscribeToEventBlah(events);
            events.AssertWasCalled(x => x.Blah += Arg<EventHandler>.Is.Anything);
        }
    }
}
