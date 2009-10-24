using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    
    public class FieldProblem_Sander
    {
        [Fact]
        public void CanUseOutIntPtr()
        {
            MockRepository mocks = new MockRepository();
            IFooWithOutIntPtr mock = mocks.StrictMock<IFooWithOutIntPtr>();
            IntPtr parameter;
            mock.GetBar(out parameter);
            LastCall.IgnoreArguments().Return(5).OutRef(new IntPtr(3));
            mocks.ReplayAll();
            Assert.Equal(5, mock.GetBar(out parameter));
            Assert.Equal(new IntPtr(3), parameter);
            mocks.VerifyAll();
        }
    }

    public interface IFooWithOutIntPtr
    {
        int GetBar(out IntPtr parameter);
    }
}