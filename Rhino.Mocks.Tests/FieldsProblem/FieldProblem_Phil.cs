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
using System.Text;
using Xunit;
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


	
	public class FieldProblem_Phil
	{

		[Fact]
		public void VerifyingThatEventWasAttached()
		{
			MockRepository mocks = new MockRepository();
			IWithEvent events = (IWithEvent)mocks.StrictMock(typeof(IWithEvent));
			events.Load += null; //ugly syntax, I know, but the only way to get this to work
			IEventRaiser raiser = LastCall.IgnoreArguments().GetEventRaiser();
			mocks.ReplayAll();

			EventConsumer consumerMock = new EventConsumer(events);
			//Next line invokes Load event.
			raiser.Raise(this, EventArgs.Empty);
			mocks.VerifyAll();

			Assert.True(consumerMock.OnLoadCalled);
		}
	}
}
