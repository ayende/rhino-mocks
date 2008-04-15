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
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Tests.FieldsProblem;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class BackToRecord
    {
        [Test]
        public void CanMoveToRecordAndThenReplay()
        {
            MockRepository mocks = new MockRepository();
            IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
            Expect.Call(demo.Prop).Return("ayende");
            mocks.Replay(demo);
            Assert.AreEqual("ayende", demo.Prop);
            mocks.BackToRecord(demo);
            Expect.Call(demo.Prop).Return("rahien");
            mocks.Replay(demo);
            Assert.AreEqual("rahien", demo.Prop);
            mocks.VerifyAll();
        }

        [Test]
        public void CanMoveToRecordFromVerified()
        {
            MockRepository mocks = new MockRepository();
            IDemo demo = (IDemo)mocks.CreateMock(typeof(IDemo));
            Expect.Call(demo.Prop).Return("ayende");
            
            mocks.Replay(demo);
            Assert.AreEqual("ayende", demo.Prop);
            mocks.VerifyAll();

            mocks.BackToRecord(demo);

            Expect.Call(demo.Prop).Return("rahien");
            mocks.Replay(demo);
            Assert.AreEqual("rahien", demo.Prop);
            mocks.VerifyAll();
        }
    
        [Test]
        public void CanSpecifyClearOnlyEvents()
        {
            MockRepository mocks = new MockRepository();
            IWithEvent withEvent = mocks.CreateMock<IWithEvent>();
            bool called = false;
            withEvent.Load += delegate { called = true; };
            IEventRaiser raiser = LastCall.GetEventRaiser();
            mocks.BackToRecord(withEvent, BackToRecordOptions.EventSubscribers);

            raiser.Raise(this, EventArgs.Empty);

            Assert.IsFalse(called);
        }

        [Test]
        [ExpectedException(typeof(ExpectationViolationException), "AbstractClass.Add(5); Expected #0, Actual #1.")]
        public void CanClearOnlyOriginalMethodCalls()
        {
            MockRepository mocks = new MockRepository();
            AbstractClass abstractClass = mocks.CreateMock<AbstractClass>();
            Expect.Call(abstractClass.Add(0)).CallOriginalMethod(OriginalCallOptions.NoExpectation);
            mocks.BackToRecord(abstractClass, BackToRecordOptions.OriginalMethodsToCall);
            mocks.ReplayAll();

            abstractClass.Add(5);
        }

        [Test]
        [ExpectedException(typeof(ExpectationViolationException),
           "IDemo.get_Prop(); Expected #0, Actual #1.")]
        public void CanClearOnlyPropertyBehavior()
        {
            MockRepository mocks = new MockRepository();
            IDemo mock = mocks.CreateMock<IDemo>();
            Expect.Call(mock.Prop).PropertyBehavior();

            mocks.BackToRecord(mock,BackToRecordOptions.PropertyBehavior);

            mocks.ReplayAll();

            string prop = mock.Prop;
        }

        [Test]
        [ExpectedException(typeof(ExpectationViolationException),
           "IDemo.VoidNoArgs(); Expected #1, Actual #0.")]
        public void CanMoveToRecordFromReplyWithoutClearingExpectations()
        {
            MockRepository mocks = new MockRepository();
            IDemo mock = mocks.CreateMock<IDemo>();

            mock.VoidNoArgs();
            mocks.ReplayAll();

            mocks.BackToRecord(mock, BackToRecordOptions.None);
            
            mock.VoidNoArgs();
            mocks.ReplayAll();

            mock.VoidNoArgs();

            mocks.VerifyAll();
        }

        [Test]
        public void CanMoveToRecordFromVerifiedWithoutClearingExpectations()
        {
            MockRepository mocks = new MockRepository();
            IDemo mock = mocks.CreateMock<IDemo>();

            mock.VoidNoArgs();
            mocks.ReplayAll();

            mock.VoidNoArgs();
            mocks.VerifyAll();

            mocks.BackToRecord(mock, BackToRecordOptions.None);
            mock.VoidNoArgs();
            mocks.ReplayAll();

            mock.VoidNoArgs();
            mocks.VerifyAll();
        }
    }
}
