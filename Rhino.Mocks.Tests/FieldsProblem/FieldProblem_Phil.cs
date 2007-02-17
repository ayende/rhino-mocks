using System;
using System.Text;
using MbUnit.Framework;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public interface IWithEvent
	{
		event EventHandler Load;
	}

	public class EventConsumer
	{
		public bool OnLoadCalled = false;

		IWithEvent _withEvent;
		public EventConsumer(IWithEvent withEvent)
		{
			_withEvent = withEvent;
			_withEvent.Load += new EventHandler(OnLoad);
		}

		void OnLoad(object sender, EventArgs e)
		{
			OnLoadCalled = true;

		}
	}


	[TestFixture]
	public class FieldProblem_Phil
	{

		[Test]
		public void VerifyingThatEventWasAttached()
		{
			MockRepository mocks = new MockRepository();
			IWithEvent events = (IWithEvent)mocks.CreateMock(typeof(IWithEvent));
			events.Load += null; //ugly syntax, I know, but the only way to get this to work
			IEventRaiser raiser = LastCall.IgnoreArguments().GetEventRaiser();
			mocks.ReplayAll();

			EventConsumer consumerMock = new EventConsumer(events);
			//Next line invokes Load event.
			raiser.Raise(this, EventArgs.Empty);
			mocks.VerifyAll();

			Assert.IsTrue(consumerMock.OnLoadCalled);
		}
	}
}
