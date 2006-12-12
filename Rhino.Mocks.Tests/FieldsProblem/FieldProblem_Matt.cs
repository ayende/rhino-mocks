//#if dotNet2
//using System;
//using System.Collections.Generic;
//using System.Text;
//using NUnit.Framework;

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