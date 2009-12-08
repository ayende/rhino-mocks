using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace RhinoMocksPartialMocks
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class RhinoMocksPartialMocksTest
    {
        [Test]
        public void UsingPartialMocks()
        {
            MockRepository mocks = new MockRepository();
            ProcessorBase proc = mocks.PartialMock<ProcessorBase>();
            Expect.Call(proc.Add(1)).Return(1);
            Expect.Call(proc.Add(1)).Return(2);
            mocks.ReplayAll();
            proc.Inc();
            Assert.AreEqual(1, proc.Register);
            proc.Inc();
            Assert.AreEqual(2, proc.Register);
            mocks.VerifyAll();
        }

        [Test]
        public void UsingPartialMocks_AAA()
        {
            //Arrange
            ProcessorBase proc = MockRepository.GeneratePartialMock<ProcessorBase>();
            proc.Expect(p => p.Add(1)).Return(1);
            proc.Expect(p => p.Add(1)).Return(2);

            //Act
            proc.Inc();
            int step1 = proc.Register;
            proc.Inc();
            int step2 = proc.Register;

            //Assert
            Assert.AreEqual(1, step1);
            Assert.AreEqual(2, step2);
            proc.VerifyAllExpectations();
        }
    }
}
