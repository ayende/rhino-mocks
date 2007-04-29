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

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_AviOrdering
    {

        public interface ISumbition
        {
            long UserID { get; set; }
            string Name { get; set; }
            string Address { get; set; }
            void Save();
        }

        public interface IView
        {
            long UserID { get; set; }
            string Name { get; set; }
            string Address { get; set; }
        }

        public class Presneter
        {
            private readonly ISumbition submition;
            private readonly IView view;

            public Presneter(IView view, ISumbition submition)
            {
                this.submition = submition;
                this.view = view;
            }

            public void Sumbit()
            {
                submition.Address = view.Address;
                submition.Name = view.Name;
                submition.UserID = view.UserID;

                submition.Save();
            }

        }

        [Test]
        public void SubmitDataToDB()
        {
            //Setup a mock view and ISumbition
            MockRepository mocks = new MockRepository();
            IView myMockView = (IView) mocks.DynamicMock(typeof (IView));
            ISumbition myMockSubmition = (ISumbition) mocks.DynamicMock(typeof (ISumbition));

            //Record expectations
            SetupResult.For(myMockView.UserID).Return(3105596L);
            SetupResult.For(myMockView.Name).Return("Someone");
            SetupResult.For(myMockView.Address).Return("Somewhere");

            using (mocks.Ordered())
            {
                using (mocks.Unordered())
                {
                    myMockSubmition.Name = "Someone";
                    myMockSubmition.Address = "Somewhere";
                    myMockSubmition.UserID = 3105596L;
                }
                myMockSubmition.Save();
            }
            
            //setup the present
            mocks.ReplayAll();
            
            Presneter myPresenter = new Presneter(myMockView, myMockSubmition);
            myPresenter.Sumbit();
            
            mocks.VerifyAll();
        }
    }

}