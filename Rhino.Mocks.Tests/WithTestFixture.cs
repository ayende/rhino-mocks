#if dotNet2
using System;
using NUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class WithTestFixture
    {
        [Test]
        public void SetupMyOwnRepository()
        {
            MockRepository mocks = new MockRepository();
            IDemo demo = mocks.CreateMock<IDemo>();
            With.Mocks(mocks,delegate
            {
                Expect.Call(demo.ReturnStringNoArgs()).Return("Hi");
                Mocker.Current.ReplayAll();
                Assert.AreEqual("Hi", demo.ReturnStringNoArgs());
            });
        }
        [Test]
        public void UsingTheWithMocksConstruct()
        {
            With.Mocks(delegate
            {
                IDemo demo = Mocker.Current.CreateMock<IDemo>();
                Expect.Call(demo.ReturnIntNoArgs()).Return(5);
                Mocker.Current.ReplayAll();
                Assert.AreEqual(5, demo.ReturnIntNoArgs());
            });
        }
        
        [Test]
        [ExpectedException(typeof(ExpectationViolationException), "IDemo.ReturnIntNoArgs(); Expected #1, Actual #0.")]
        public void UsingTheWithMocksConstruct_ThrowsIfExpectationIsMissed()
        {
            With.Mocks(delegate
            {
                IDemo demo = Mocker.Current.CreateMock<IDemo>();
                Expect.Call(demo.ReturnIntNoArgs()).Return(5);
                Mocker.Current.ReplayAll();
            });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "This action is invalid when the mock object is in record state.")]
        public void UsingTheWithMocksConstruct_ThrowsIfReplayAllNotCalled()
        {
            With.Mocks(delegate
            {
                IDemo demo = Mocker.Current.CreateMock<IDemo>();
                Expect.Call(demo.ReturnIntNoArgs()).Return(5);
            });
        }


        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException), "foo")]
        public void UsingTheWithMocksConstruct_GiveCorrectExceptionWhenMocking()
        {
            With.Mocks(delegate
            {
                IDemo demo = Mocker.Current.CreateMock<IDemo>();
                Expect.Call(demo.ReturnIntNoArgs()).Return(5);
                Mocker.Current.ReplayAll();
                throw new IndexOutOfRangeException("foo");
            });
        }


        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException), "foo")]
        public void UsingTheWithMocksConstruct_GiveCorrectExceptionWhenMockingEvenIfReplayAllNotCalled()
        {
            With.Mocks(delegate
            {
                IDemo demo = Mocker.Current.CreateMock<IDemo>();
                Expect.Call(demo.ReturnIntNoArgs()).Return(5);
                throw new IndexOutOfRangeException("foo");
            });
        }
    }
}

#endif