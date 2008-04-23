using System;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Sander
    {
        [Test]
        public void CanUseOutIntPtr()
        {
            MockRepository mocks = new MockRepository();
            IFooWithOutIntPtr mock = mocks.CreateMock<IFooWithOutIntPtr>();
            IntPtr parameter;
            mock.GetBar(out parameter);
            LastCall.IgnoreArguments().Return(5).OutRef(new IntPtr(3));
            mocks.ReplayAll();
            Assert.AreEqual(5, mock.GetBar(out parameter));
            Assert.AreEqual(new IntPtr(3), parameter);
            mocks.VerifyAll();
        }
    }

    public interface IFooWithOutIntPtr
    {
        int GetBar(out IntPtr parameter);
    }
}