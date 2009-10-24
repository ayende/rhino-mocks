#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using Xunit;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	
	public class UsingEvents
	{
		MockRepository mocks;

		public UsingEvents()
		{
			mocks = new MockRepository();
		}

		[Fact]
		public void VerifyingThatEventWasAttached()
		{
			IWithEvents events = (IWithEvents)mocks.StrictMock(typeof(IWithEvents));
			events.Blah += new EventHandler(events_Blah);
			mocks.ReplayAll();
			MethodThatSubscribeToEventBlah(events);
			mocks.VerifyAll();

		}

		public void MethodThatSubscribeToEventBlah(IWithEvents events)
		{
			events.Blah += new EventHandler(events_Blah);
		}

		[Fact]
		public void VerifyingThatAnEventWasFired()
		{
			IEventSubscriber subscriber = (IEventSubscriber)mocks.StrictMock(typeof(IEventSubscriber));
			IWithEvents events = new WithEvents();
			// This doesn't create an expectation because no method is called on subscriber!!
			events.Blah += new EventHandler(subscriber.Hanlder);
			subscriber.Hanlder(events, EventArgs.Empty);
			mocks.ReplayAll();
			events.RaiseEvent();
			mocks.VerifyAll();

		}

		[Fact]
		public void VerifyingThatAnEventWasFiredThrowsForDifferentArgument()
		{
			MockRepository mocks = new MockRepository();

			IEventSubscriber subscriber = (IEventSubscriber)mocks.StrictMock(typeof(IEventSubscriber));
			IWithEvents events = new WithEvents();
			// This doesn't create an expectation because no method is called on subscriber!!
			events.Blah += new EventHandler(subscriber.Hanlder);
			subscriber.Hanlder(events, new EventArgs());
			mocks.ReplayAll();
			
			Assert.Throws<ExpectationViolationException>(
				"IEventSubscriber.Hanlder(Rhino.Mocks.Tests.FieldsProblem.WithEvents, System.EventArgs); Expected #0, Actual #1.\r\nIEventSubscriber.Hanlder(Rhino.Mocks.Tests.FieldsProblem.WithEvents, System.EventArgs); Expected #1, Actual #0.",
				() => events.RaiseEvent());
		}

		[Fact]
		public void CanSetExpectationToUnsubscribeFromEvent()
		{
			IWithEvents events = (IWithEvents)mocks.StrictMock(typeof(IWithEvents));
			events.Blah += new EventHandler(events_Blah);
			events.Blah -= new EventHandler(events_Blah);
			mocks.ReplayAll();
			events.Blah += new EventHandler(events_Blah);
			events.Blah -= new EventHandler(events_Blah);
			mocks.VerifyAll();
		}

		[Fact]
		public void VerifyingExceptionIfEventIsNotAttached()
		{
			IWithEvents events = (IWithEvents)mocks.StrictMock(typeof(IWithEvents));
			events.Blah += new EventHandler(events_Blah);
			mocks.ReplayAll();
			Assert.Throws<ExpectationViolationException>("IWithEvents.add_Blah(System.EventHandler); Expected #1, Actual #0.",
			                                             () => mocks.VerifyAll());

		}

		[Fact]
		public void VerifyingThatCanAttackOtherEvent()
		{
			IWithEvents events = (IWithEvents)mocks.StrictMock(typeof(IWithEvents));
			events.Blah += new EventHandler(events_Blah);
			LastCall.IgnoreArguments();
			mocks.ReplayAll();
			events.Blah += new EventHandler(events_Blah_Other);
			mocks.VerifyAll();

		}

		[Fact]
		public void BetterErrorMessageOnIncorrectParametersCount()
		{
			IWithEvents events = (IWithEvents)mocks.StrictMock(typeof(IWithEvents));
			events.Blah += null;
			raiser = LastCall.IgnoreArguments().GetEventRaiser();
			mocks.ReplayAll();
			events.Blah += delegate { };
			
			Assert.Throws<InvalidOperationException>(
				"You have called the event raiser with the wrong number of parameters. Expected 2 but was 0",
				() => raiser.Raise(null));
		}

		[Fact]
		public void BetterErrorMessageOnIncorrectParameters()
		{
			IWithEvents events = (IWithEvents)mocks.StrictMock(typeof(IWithEvents));
			events.Blah += null;
			raiser = LastCall.IgnoreArguments().GetEventRaiser();
			mocks.ReplayAll();
			events.Blah += delegate { };
			Assert.Throws<InvalidOperationException>(
				"Parameter #2 is System.Int32 but should be System.EventArgs",
				() => raiser.Raise("", 1));
		}

		private void events_Blah_Other(object sender, EventArgs e)
		{
		}

		private void events_Blah(object sender, EventArgs e)
		{
		}

		IEventRaiser raiser;

		[Fact]
		public void RaiseEvent()
		{
			IWithEvents eventHolder = (IWithEvents)mocks.StrictMock(typeof(IWithEvents));
			eventHolder.Blah += null;
			LastCall.IgnoreArguments();
			raiser = LastCall.GetEventRaiser();
			eventHolder.RaiseEvent();
			LastCall.Do(new System.Threading.ThreadStart(UseEventRaiser));
			IEventSubscriber eventSubscriber = (IEventSubscriber)mocks.StrictMock(typeof(IEventSubscriber));
			eventSubscriber.Hanlder(this, EventArgs.Empty);

			mocks.ReplayAll();

			eventHolder.Blah += new EventHandler(eventSubscriber.Hanlder);

			eventHolder.RaiseEvent();

			mocks.VerifyAll();
		}

		[Fact]
		public void UsingEventRaiserCreate()
		{
			IWithEvents eventHolder = (IWithEvents)mocks.Stub(typeof(IWithEvents));
			IEventRaiser eventRaiser = EventRaiser.Create(eventHolder, "Blah");
			bool called = false;
			eventHolder.Blah += delegate
			{
				called = true;
			};

			mocks.ReplayAll();

			eventRaiser.Raise(this, EventArgs.Empty);

			mocks.VerifyAll();

			Assert.True(called);
		}

		private void UseEventRaiser()
		{
			raiser.Raise(this, EventArgs.Empty);
		}

#if DOTNET35
		
        [Fact]
        public void RaiseEventUsingExtensionMethod() 
        {
            IWithEvents eventHolder = (IWithEvents)mocks.Stub(typeof(IWithEvents));
            bool called = false;
            eventHolder.Blah += delegate {
                called = true;
            };
            
            eventHolder.Raise(stub => stub.Blah += null, this, EventArgs.Empty);
            
            Assert.True(called);
        }

        [Fact]
        public void UsingEventRaiserFromExtensionMethod() 
        {
            IWithEvents eventHolder = (IWithEvents)mocks.Stub(typeof(IWithEvents));
            IEventRaiser eventRaiser = eventHolder.GetEventRaiser(stub => stub.Blah += null);
			
			mocks.ReplayAll();
			
			bool called = false;
            eventHolder.Blah += delegate {
                called = true;
            };


            eventRaiser.Raise(this, EventArgs.Empty);

            mocks.VerifyAll();

            Assert.True(called);
		}
#endif

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
