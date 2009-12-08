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
            IProjectView view = mocks.StrictMock<IProjectView>();
            Project prj = new Project("Example Project");
            IProjectPresenter presenter = new ProjectPresenter(prj, view);

            Expect.Call(view.Title).Return(prj.Name);
            Expect.Call(view.Ask(null, null)).Return("a");

            mocks.ReplayAll();
            Assert.IsFalse(presenter.SaveProjectAs());
            mocks.VerifyAll();
        }

        /// <summary>
        /// This Test is the same than above using Arrange/Act/Assert pattern
        /// </summary>
        [Test]
        public void SaveProjectAs_CanBeCanceled_AAA()
        {
            //Arrange
            IProjectView view = MockRepository.GenerateStrictMock<IProjectView>();
            Project prj = new Project("Example Project");
            IProjectPresenter presenter = new ProjectPresenter(prj, view);

            view.Expect(v => v.Title).Return(null);
            view.Expect(v => v.Ask(Arg<string>.Is.Null, Arg<string>.Is.Null)).Return(null);

            //Act
            bool isProjectSave = presenter.SaveProjectAs();

            //Assert
            Assert.IsFalse(isProjectSave);
            view.VerifyAllExpectations();
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
        [ExpectedException(typeof(ExpectationViolationException),
            ExpectedMessage = "IDemo.VoidNoArgs(); Expected #0, Actual #1.")]
        public void MockObjectThrowsForUnexpectedCall_AAA()
        {
            //Arrange
            IDemo demo = MockRepository.GenerateStrictMock<IDemo>();

            //Act
            demo.VoidNoArgs();

            //Assert is a different, we expect an exception
        }

        [Test]
        public void DyamicMockAcceptUnexpectedCall()
        {
            MockRepository mocks = new MockRepository();
            IDemo demo = mocks.DynamicMock<IDemo>();

            mocks.ReplayAll();
            demo.VoidNoArgs();
            Assert.Pass();
            mocks.VerifyAll();//works like a charm
        }

        [Test]
        public void DyamicMockAcceptUnexpectedCall_AAA()
        {
            //Arrange
            IDemo demo = MockRepository.GenerateMock<IDemo>();

            //Act
            demo.VoidNoArgs();

            //Assert
            Assert.Pass();
        }
    }
}
