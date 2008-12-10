using System;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_LAFAY
    {
        private IDemo demo;
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            demo = mocks.StrictMock(typeof (IDemo)) as IDemo;
        }

        [Test]
        public void ExpectTwoCallsReturningMarshalByRef()
        {
            var res1 = new MarshalByRefToReturn();
            var res2 = new MarshalByRefToReturn();
            Expect.Call(demo.ReturnMarshalByRefNoArgs()).Return(res1);
            Expect.Call(demo.ReturnMarshalByRefNoArgs()).Return(res2);
            mocks.ReplayAll();
            demo.ReturnMarshalByRefNoArgs();
            demo.ReturnMarshalByRefNoArgs();
        }

        #region Nested type: IDemo

        public interface IDemo
        {
            MarshalByRefToReturn ReturnMarshalByRefNoArgs();
        }

        #endregion

        #region Nested type: MarshalByRefToReturn

        public class MarshalByRefToReturn : MarshalByRefObject
        {
            public override string ToString()
            {
                return "test";
            }
        }

        #endregion
    }
}