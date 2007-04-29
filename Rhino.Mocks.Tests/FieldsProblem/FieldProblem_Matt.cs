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


//#if dotNet2
//using System;
//using System.Collections.Generic;
//using System.Text;
//using MbUnit.Framework;

//namespace Rhino.Mocks.Tests.FieldsProblem
//{
//    [TestFixture]
//    public class FieldProblem_Matt
//    {

//        public interface IModel
//        {
//            void Add(string s);
//            void Clear();

//            event EventHandler ModelChanged;
//        }

//        public class MyModel : IModel
//        {

//            #region IModel Members

//            public virtual void Add(string s)
//            {
//                throw new Exception("The method or operation is not implemented.");
//            }

//            public virtual void Clear()
//            {
//                throw new Exception("The method or operation is not implemented.");
//            }


//            #endregion
//        }
//        public interface IView
//        {
//            void SetList(params string[] s);
//        }

//        public class Presenter
//        {
//            private IModel model;
//            private IView view;

//            public Presenter(IView view, IModel model)
//            {
//                this.model = model;
//                this.view = view;

//                this.model.ModelChanged += new EventHandler(ModelChangedListener);
//            }

//            private void ModelChangedListener(object sender, EventArgs e)
//            {
//                view.SetList("foo");
//            }
//        }

//        [Test]
//        public void ClearedModelSetsItemsOnView()
//        {
//            MockRepository mocks = new MockRepository();
//            IModel model = mocks.CreateMock<IModel>();
//            IView view = mocks.CreateMock<IView>();
//            model.ModelChanged += null;
//            LastCall.IgnoreArguments();
//            IEventRaiser eventRaiser = LastEvent.GetRaiser();

//            view.SetList(null);
//            LastCall.IgnoreArguments();
//            mocks.ReplayAll();

//            Presenter subject = new Presenter(view, model);

//            eventRaiser.Raise(this, EventArgs.Empty);

//            mocks.VerifyAll();
//        }

//    }
//}
//#endif