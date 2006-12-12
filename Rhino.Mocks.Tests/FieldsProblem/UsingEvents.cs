using System;
using NUnit.Framework;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class UsingEvents
    {
        MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void VerifyingThatEventWasAttached()
        {
            IWithEvents events = (IWithEvents)mocks.CreateMock(typeof(IWithEvents));
            events.Blah += new EventHandler(events_Blah);
            mocks.ReplayAll();
            MethodThatSubscribeToEventBlah(events);
            mocks.VerifyAll();

        }

        public void MethodThatSubscribeToEventBlah(IWithEvents events)
        {
            events.Blah += new EventHandler(events_Blah);
        }

        [Test]
        public void VerifyingThatAnEventWasFired()
        {
            IEventSubscriber subscriber = (IEventSubscriber)mocks.CreateMock(typeof(IEventSubscriber));
            IWithEvents events = new WithEvents();
            // This doesn't create an expectation because no method is called on subscriber!!
            events.Blah += new EventHandler(subscriber.Hanlder);
            subscriber.Hanlder(events, EventArgs.Empty);
            mocks.ReplayAll();
            events.RaiseEvent();
            mocks.VerifyAll();

        }

        [Test]
        [ExpectedException(typeof(ExpectationViolationException), "IEventSubscriber.Hanlder(Rhino.Mocks.Tests.FieldsProblem.WithEvents, System.EventArgs); Expected #0, Actual #1.\r\nIEventSubscriber.Hanlder(Rhino.Mocks.Tests.FieldsProblem.WithEvents, System.EventArgs); Expected #1, Actual #0.")]
        public void VerifyingThatAnEventWasFiredThrowsForDifferentArgument()
        {
            MockRepository mocks = new MockRepository();

            IEventSubscriber subscriber = (IEventSubscriber)mocks.CreateMock(typeof(IEventSubscriber));
            IWithEvents events = new WithEvents();
            // This doesn't create an expectation because no method is called on subscriber!!
            events.Blah += new EventHandler(subscriber.Hanlder);
            subscriber.Hanlder(events, new EventArgs());
            mocks.ReplayAll();
            events.RaiseEvent();
            mocks.VerifyAll();

        }

        [Test]
        [ExpectedException(typeof(ExpectationViolationException), "IWithEvents.add_Blah(System.EventHandler); Expected #1, Actual #0.")]
        public void VerifyingExceptionIfEventIsNotAttached()
        {
            IWithEvents events = (IWithEvents)mocks.CreateMock(typeof(IWithEvents));
            events.Blah += new EventHandler(events_Blah);
            mocks.ReplayAll();
            mocks.VerifyAll();

        }

        [Test]
        public void VerifyingThatCanAttackOtherEvent()
        {
            IWithEvents events = (IWithEvents)mocks.CreateMock(typeof(IWithEvents));
            events.Blah += new EventHandler(events_Blah);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            events.Blah += new EventHandler(events_Blah_Other);
            mocks.VerifyAll();

        }

        private void events_Blah_Other(object sender, EventArgs e)
        {
        }

        private void events_Blah(object sender, EventArgs e)
        {
        }

        IEventRaiser raiser;

        [Test]
        public void RaiseEvent()
        {
            IWithEvents eventHolder = (IWithEvents)mocks.CreateMock(typeof(IWithEvents));
            eventHolder.Blah += null;
            LastCall.IgnoreArguments();
            raiser = LastCall.GetEventRaiser();
            eventHolder.RaiseEvent();
            LastCall.Do(new System.Threading.ThreadStart(UseEventRaiser));
            IEventSubscriber eventSubscriber = (IEventSubscriber)mocks.CreateMock(typeof(IEventSubscriber));
            eventSubscriber.Hanlder(this, EventArgs.Empty);
            
            mocks.ReplayAll();

            eventHolder.Blah += new EventHandler(eventSubscriber.Hanlder);

            eventHolder.RaiseEvent();
            
            mocks.VerifyAll();
        }

        private void UseEventRaiser()
        {
            raiser.Raise(this, EventArgs.Empty);
        }
    }

    public interface IWithEvents
    {
        event EventHandler Blah;
        void RaiseEvent();
    }

    public interface IEventSubscriber
    {
        void Hanlder(object sender, EventArgs e);
    }

    public class WithEvents : IWithEvents
    {
        #region IWithEvents Members

        public event System.EventHandler Blah;

        public void RaiseEvent()
        {
            if (Blah != null)
                Blah(this, EventArgs.Empty);
        }

        #endregion
    }

}
