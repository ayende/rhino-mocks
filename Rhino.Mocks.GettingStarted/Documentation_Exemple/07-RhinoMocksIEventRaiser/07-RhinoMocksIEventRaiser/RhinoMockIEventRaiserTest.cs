using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace RhinoMocksIEventRaiser
{
    /// <summary>
    /// The event raiser is a solution to a common problem, 
    /// how do you raise an event from an interface? Let us consider this code:
    /// </summary>
    /// <remarks>
    /// Note: Event Subscribers, like all other information on a mock object, 
    /// will be cleared if BackToRecord(mock) or BackToRecordAll() are called.
    /// </remarks>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+IEventRaiser.ashx"/>
    public class RhinoMockIEventRaiserTest
    {
        /// <summary>
        /// Getting IEventRaiser Implementation
        /// </summary>
        [Test]
        public void RaisingEventOnView()
        {
           MockRepository mocks = new MockRepository();
           IView view = mocks.StrictMock<IView>();
           view.Load += null;//create an expectation that someone will subscribe to this event
           LastCall.IgnoreArguments();// we don't care who is subscribing
           IEventRaiser raiseViewEvent = LastCall.GetEventRaiser();//get event raiser for the last event, in this case, View
        }

        /// <summary>
        /// How to raise an event and test it
        /// </summary>
        [Test]
        public void RaisingEventOnViewTest()
        {
           MockRepository mocks = new MockRepository();
           IView view = mocks.StrictMock<IView>();
           view.Load += null;//create an expectation that someone will subscribe to this event
           LastCall.IgnoreArguments();// we don't care who is subscribing
           IEventRaiser raiseViewEvent = LastCall.GetEventRaiser();//get event raiser for the last event, in this case, View

           mocks.ReplayAll();

           Presenter p = new Presenter(view);
           raiseViewEvent.Raise(this, new EventArgs());//Raise the event which has been bind then Present is instanciating

           Assert.IsTrue(p.OnLoadCalled);   
        }
    }
}
