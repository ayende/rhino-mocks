using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;

namespace RhinoMocksGenerics
{
    /// <summary>
    /// While Rhino Mocks is compatible with the .Net framework 1.1,
    /// it offer several goodies for those who use the 2.0 version of the .Net framework.
    /// 
    /// Among them are mocking generic interfaces and classes,
    /// and using generic version of the methods in order to reduce casting.
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+Generics.ashx"/>
    [TestFixture]
    public class RhinoMocksGenericsTest
    {
        [Test]
        public void MockAGenericInterface()
        {
            MockRepository mocks = new MockRepository();
            IList<int> list = mocks.StrictMock<IList<int>>();

            Expect.Call(list.Count).Return(5);

            mocks.ReplayAll();
            Assert.AreEqual(5, list.Count);
            mocks.VerifyAll();
        }

        [Test]
        public void MockAGenericInterface_AAA()
        {
            //Arrange
            IList<int> list = MockRepository.GenerateStrictMock<IList<int>>();
            list.Expect(l => l.Count).Return(5);

            //Act
            int count = list.Count;

            //Assert
            Assert.AreEqual(5, count);
            list.VerifyAllExpectations();
        } 
    }
}
