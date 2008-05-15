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
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Impl;
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

		[Test]
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

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IEventSubscriber.Hanlder(Rhino.Mocks.Tests.FieldsProblem.WithEvents, System.EventArgs); Expected #0, Actual #1.\r\nIEventSubscriber.Hanlder(Rhino.Mocks.Tests.FieldsProblem.WithEvents, System.EventArgs); Expected #1, Actual #0.")]
		public void VerifyingThatAnEventWasFiredThrowsForDifferentArgument()
		{
			MockRepository mocks = new MockRepository();

			IEventSubscriber subscriber = (IEventSubscriber)mocks.StrictMock(typeof(IEventSubscriber));
			IWithEvents events = new WithEvents();
			// This doesn't create an expectation because no method is called on subscriber!!
			events.Blah += new EventHandler(subscriber.Hanlder);
			subscriber.Hanlder(events, new EventArgs());
			mocks.ReplayAll();
			events.RaiseEvent();
			mocks.VerifyAll();
		}

		[Test]
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

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IWithEvents.add_Blah(System.EventHandler); Expected #1, Actual #0.")]
		public void VerifyingExceptionIfEventIsNotAttached()
		{
			IWithEvents events = (IWithEvents)mocks.StrictMock(typeof(IWithEvents));
			events.Blah += new EventHandler(events_Blah);
			mocks.ReplayAll();
			mocks.VerifyAll();

		}

		[Test]
		public void VerifyingThatCanAttackOtherEvent()
		{
			IWithEvents events = (IWithEvents)mocks.StrictMock(typeof(IWithEvents));
			events.Blah += new EventHandler(events_Blah);
			LastCall.IgnoreArguments();
			mocks.ReplayAll();
			events.Blah += new EventHandler(events_Blah_Other);
			mocks.VerifyAll();

		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),
		   "You have called the event raiser with the wrong number of parameters. Expected 2 but was 0")]
		public void BetterErrorMessageOnIncorrectParametersCount()
		{
			IWithEvents events = (IWithEvents)mocks.StrictMock(typeof(IWithEvents));
			events.Blah += null;
			raiser = LastCall.IgnoreArguments().GetEventRaiser();
			mocks.ReplayAll();
			events.Blah += delegate { };
			raiser.Raise(null);
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),
		  "Parameter #2 is System.Int32 but should be System.EventArgs")]
		public void BetterErrorMessageOnIncorrectParameters()
		{
			IWithEvents events = (IWithEvents)mocks.StrictMock(typeof(IWithEvents));
			events.Blah += null;
			raiser = LastCall.IgnoreArguments().GetEventRaiser();
			mocks.ReplayAll();
			events.Blah += delegate { };
			raiser.Raise("",1);
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

		[Test]
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

			Assert.IsTrue(called);
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
