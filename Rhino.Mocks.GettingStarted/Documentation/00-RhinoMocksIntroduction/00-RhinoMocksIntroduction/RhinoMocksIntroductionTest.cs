using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Exceptions;

namespace RhinoMocksIntroduction
{
    /// <summary>
    /// The purpose of mock objects is to allow you
    /// to test the interactions between components,
    /// this is very useful when you test code
    /// that doesn't lend itself easily to state based testing.
    /// 
    /// Most of the examples here are tests that check the save routine for a view
    /// to see if it is working as expected.
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+Introduction.ashx"/>
    public class RhinoMocksIntroductionTest
    {
        [Test]
        public void SaveProjectAs_CanBeCanceled()
        {
            MockRepository mocks = new MockRepository();
            IProjectView projectView = mocks.StrictMock<IProjectView>();

            Project prj = new Project("Example Project");
            IProjectPresenter presenter = new ProjectPresenter(prj, projectView);
            Expect.Call(projectView.Title).Return(prj.Name);
            Expect.Call(projectView.Ask(null, null)).Return(null);

            mocks.ReplayAll();
            Assert.IsFalse(presenter.SaveProjectAs());
            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(ExpectationViolationException),
            ExpectedMessage = "IDemo.VoidNoArgs(); Expected #0, Actual #1.")]
        public void MockObjectThrowsForUnexpectedCall()
        {
            MockRepository mocks = new MockRepository();
            IDemo demo = mocks.StrictMock<IDemo>();
            mocks.ReplayAll();
            demo.VoidNoArgs();
            mocks.VerifyAll();//will never get here
        }

        [Test]
        public void DyamicMockAcceptUnexpectedCall()
        {
            MockRepository mocks = new MockRepository();
            IDemo demo = mocks.DynamicMock<IDemo>();
            mocks.ReplayAll();
            demo.VoidNoArgs();
            mocks.VerifyAll();//works like a charm
        }
    }
}
