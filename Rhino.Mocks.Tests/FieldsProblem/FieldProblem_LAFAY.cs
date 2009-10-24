using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    
    public class FieldProblem_LAFAY
    {
        private IDemo demo;
        private MockRepository mocks;

		public FieldProblem_LAFAY()
        {
            mocks = new MockRepository();
            demo = mocks.StrictMock(typeof (IDemo)) as IDemo;
        }

        [Fact]
        public void ExpectTwoCallsReturningMarshalByRef()
        {
            MarshalByRefToReturn res1 = new MarshalByRefToReturn();
            MarshalByRefToReturn res2 = new MarshalByRefToReturn();
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