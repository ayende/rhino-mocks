using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Rhino.Mocks;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldsProblem_Royston
    {
        private MockRepository mMocks;
        [SetUp]
        public void SetUp()
        {
            mMocks = new MockRepository();
        }

		public interface IDuplicateType<T>
		{
			int Property { get; }
		}

		[Test]
		public void DuplicateTypeTest()
		{
			// Let's just create two mocks of the same type, based on
			// an array type parameter.

			// This should not blow up.

			IDuplicateType<object[]> mock1 =
				mMocks.CreateMock<IDuplicateType<object[]>>();

			IDuplicateType<object[]> mock2 =
				mMocks.CreateMock<IDuplicateType<object[]>>();

			mMocks.ReplayAll();
			mMocks.VerifyAll();
		}


        [Test]
        public void TestVirtualEntrypoint()
        {
            IIntf1 i1 = CreateAndConfigureMock();

            mMocks.ReplayAll();

            i1.VirtualGo();

            mMocks.VerifyAll();
        }

        [Test]
        public void TestNonVirtualEntrypoint()
        {
            IIntf1 i1 = CreateAndConfigureMock();

            mMocks.ReplayAll();
            
            i1.NonVirtualGo();

            mMocks.VerifyAll();
        }

        [Test]
        public void BackToRecordProblem()
        {
            IIntf1 i1 = (IIntf1)mMocks.CreateMock(typeof(IIntf1));

            using (mMocks.Ordered())
            {
                i1.A();
                using (mMocks.Unordered())
                {
                    i1.B();
                    i1.C();
                    LastCall.Repeat.Times(1, 2);
                }
            }

            mMocks.ReplayAll();

            i1.A();
            i1.C();
            i1.B();

            mMocks.VerifyAll();

            mMocks.BackToRecord(i1);

            i1.A();
            i1.B();

            mMocks.Replay(i1);

            i1.A();
            i1.B();

            mMocks.Verify(i1);

        }

        private IIntf1 CreateAndConfigureMock()
        {
            IIntf1 i1 = (IIntf1)mMocks.PartialMock( typeof(Cls1) );

            using ( mMocks.Ordered() )
            {
                using ( mMocks.Unordered() )
                {
                    i1.A();
                    i1.B();
                }
                i1.A();
            }
            return i1;
        }

        public interface IIntf1
        {
            void A();
            void B();
            void C();
            void VirtualGo();
            void NonVirtualGo();
        }

        public abstract class Cls1 : IIntf1
        {
            public abstract void A();
            public abstract void B();
            public abstract void C();

            public virtual void VirtualGo()
            {
                A();
                B();
                A();
            }

            public void NonVirtualGo()
            {
                A();
                B();
                A();
            }
        }
    }
}
