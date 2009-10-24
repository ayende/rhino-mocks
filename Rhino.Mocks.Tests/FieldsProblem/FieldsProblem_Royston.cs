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
using Rhino.Mocks;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    
    public class FieldsProblem_Royston
    {
        private MockRepository mMocks;
		public FieldsProblem_Royston()
        {
            mMocks = new MockRepository();
        }

		public interface IDuplicateType<T>
		{
			int Property { get; }
		}

		[Fact]
		public void DuplicateTypeTest()
		{
			// Let's just create two mocks of the same type, based on
			// an array type parameter.

			// This should not blow up.

			IDuplicateType<object[]> mock1 =
				mMocks.StrictMock<IDuplicateType<object[]>>();

			IDuplicateType<object[]> mock2 =
				mMocks.StrictMock<IDuplicateType<object[]>>();

			mMocks.ReplayAll();
			mMocks.VerifyAll();
		}

        [Fact]
        public void TestVirtualEntrypoint()
        {
            IIntf1 i1 = CreateAndConfigureMock();

            mMocks.ReplayAll();

            i1.VirtualGo();

            mMocks.VerifyAll();
        }

        [Fact]
        public void TestNonVirtualEntrypoint()
        {
            IIntf1 i1 = CreateAndConfigureMock();

            mMocks.ReplayAll();
            
            i1.NonVirtualGo();

            mMocks.VerifyAll();
        }

        [Fact]
        public void BackToRecordProblem()
        {
            IIntf1 i1 = (IIntf1)mMocks.StrictMock(typeof(IIntf1));

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
