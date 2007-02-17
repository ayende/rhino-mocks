using System;
using System.EnterpriseServices;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public interface ISomething
    {
        void SomeMethod();
    }

    [TestFixture]
    public class ReproFixture
    {
        [Test]
        public void TestMethod1()
        {
            ServiceDomain.Enter(new ServiceConfig());
            MockRepository mocks = new MockRepository();
            ISomething something = (ISomething)mocks.CreateMock(typeof(ISomething));
            mocks.ReplayAll();
            mocks.VerifyAll();
            ContextUtil.SetAbort();
            ServiceDomain.Leave();
        }
    }
}
