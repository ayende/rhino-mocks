using System;
using System.Text;
using MbUnit.Framework;

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
    
    
    }
}
