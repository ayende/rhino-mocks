using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace RhinoMocksIntroduction
{
    /// <summary>
    /// Ordered is useful to verify if expectation in the code calls are done in the good order.
    /// You can control the correct callin
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+Ordered+and+Unordered.ashx"/>
    public class RhinoMocksOrderedUnorderedTest
    {
        [Test]
        public void SaveProjectAs_NewNameWithoutConflicts()
        {
            MockRepository mocks = new MockRepository();
            IProjectView view = mocks.StrictMock<IProjectView>();
            IProjectRepository repository = mocks.StrictMock<IProjectRepository>();
            IProject prj = mocks.StrictMock<IProject>();

            //Component to test
            IProjectPresenter presenter = new ProjectPresenter(prj, repository, view);

            string question = "Mock ?";
            string answer = "Yes";
            string newProjectName = "RhinoMocks";

            //Not expected but its necessary for the implementation
            //We give the property behavior to prj.Name
            Expect.Call(prj.Name).PropertyBehavior();

            using (mocks.Ordered())
            {
                Expect.Call(view.Title).
                    Return(prj.Name);
                Expect.Call(view.Ask(question, answer)).
                    Return(newProjectName);
                Expect.Call(repository.GetProjectByName(newProjectName)).
                    Return(null);

                view.Title = newProjectName;
                view.HasChanges = false;

                repository.SaveProject(prj);
            }

            mocks.ReplayAll();

            Assert.IsTrue(presenter.SaveProjectAs());
            Assert.AreEqual(newProjectName, prj.Name);

            mocks.VerifyAll();
        }

        [Test]
        public void SaveProjectAs_NewNameWithoutConflicts_AAA()
        {
            //Arrange
            MockRepository mocks = new MockRepository();
            IProjectView view = mocks.StrictMock<IProjectView>();
            IProjectRepository repository = mocks.StrictMock<IProjectRepository>();
            IProject prj = mocks.StrictMock<IProject>();

            //Component to test
            IProjectPresenter presenter = new ProjectPresenter(prj, repository, view);

            string question = "Mock ?";
            string answer = "Yes";
            string newProjectName = "RhinoMocks";

            //Not expected but its necessary for the implementation
            //We give the property behavior to prj.Name
            prj.Expect(p => p.Name).PropertyBehavior();

            using (mocks.Ordered())
            {
                view.Expect(v => v.Title).Return(prj.Name);
                view.Expect(v => v.Ask(question, answer)).Return(newProjectName);
                repository.Expect(r => r.GetProjectByName(newProjectName)).Return(null);

                view.Title = newProjectName;
                view.HasChanges = false;

                repository.SaveProject(prj);
            }
            //Act
            mocks.ReplayAll();
            bool isSave = presenter.SaveProjectAs();

            //Assert
            Assert.IsTrue(isSave);
            Assert.AreEqual(newProjectName, prj.Name);
            view.VerifyAllExpectations();
            repository.VerifyAllExpectations();
            prj.VerifyAllExpectations();
        }

        [Test]
        public void MovingFundsUsingTransactions()
        {
            MockRepository mocks = new MockRepository();
            IDatabaseManager databaseManager = mocks.StrictMock<IDatabaseManager>();
            IBankAccount accountOne = mocks.StrictMock<IBankAccount>(),
                         accountTwo = mocks.StrictMock<IBankAccount>();

            using (mocks.Ordered())
            {
                Expect.Call(databaseManager.BeginTransaction()).Return(databaseManager);
                using (mocks.Unordered())
                {
                    Expect.Call(accountOne.Withdraw(1000));
                    Expect.Call(accountTwo.Deposit(1000));
                }
                databaseManager.Dispose();
            }

            mocks.ReplayAll();

            Bank bank = new Bank(databaseManager);
            bank.TransferFunds(accountOne, accountTwo, 1000);

            mocks.VerifyAll();
        }

        [Test]
        public void MovingFundsUsingTransactions_AAA()
        {
            //Arrange
            MockRepository mocks = new MockRepository();
            IDatabaseManager databaseManager = mocks.StrictMock<IDatabaseManager>();
            IBankAccount accountOne = mocks.StrictMock<IBankAccount>(),
                         accountTwo = mocks.StrictMock<IBankAccount>();

            using (mocks.Ordered())
            {
                databaseManager.Expect(d => d.BeginTransaction()).Return(databaseManager);
                using (mocks.Unordered())
                {
                    accountOne.Expect(a => a.Withdraw(1000));
                    accountTwo.Expect(a => a.Deposit(1000));
                }
                databaseManager.Dispose();
            }

            //Act
            mocks.ReplayAll();

            Bank bank = new Bank(databaseManager);
            bank.TransferFunds(accountOne, accountTwo, 1000);

            //Assert
            databaseManager.VerifyAllExpectations();
            accountOne.VerifyAllExpectations();
            accountTwo.VerifyAllExpectations();
        }
    }
}
