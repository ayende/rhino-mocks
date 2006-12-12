using System;
using System.Text;
using NUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_BackToMockWithRepeatableMethods
    {
        [Test]
        public void UsingBackToRecordWithSetUpResult()
        {
            MockRepository mocks = new MockRepository();
            IDemo demo = (IDemo) mocks.CreateMock(typeof(IDemo));
            SetupResult.For(demo.Prop).Return("Here is 1 sample greeting");
            mocks.Replay(demo);
            Assert.AreEqual("Here is 1 sample greeting",demo.Prop);
            mocks.BackToRecord(demo);
            SetupResult.For(demo.Prop).Return("Here is another sample greeting");
            mocks.Replay(demo);
            Assert.AreEqual("Here is another sample greeting", demo.Prop);
            mocks.VerifyAll();
        }
    }
}
