using System;
using System.Text;
using NUnit.Framework;

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