using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public interface IBase
    {
        String BaseString { get; set; }
    }

    public interface IChild : IBase
    {
        String ChildString { get; set; }
    }

    [TestFixture]
    public class StubDemoTestFixture
    {

        #region Variables

        private MockRepository _mocks;
        private IBase _mockBase;
        private IChild _mockChild;

        #endregion

        #region Setup and Teardown

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();

            _mockBase = _mocks.Stub<IBase>();
            _mockChild = _mocks.Stub<IChild>();

            _mocks.ReplayAll();
        }

        [TearDown]
        public void TearDown()
        {
            _mocks.VerifyAll();
        }

        #endregion

        #region Tests

        [Test]
        public void BaseStubSetsBasePropertiesCorrectly()
        {
            String str = "Base stub";

            _mockBase.BaseString = str;

            Assert.AreEqual(str, _mockBase.BaseString);
        }

        [Test]
        public void ChildStubSetsChildPropertiesCorrectly()
        {
            String str = "Child stub";

            _mockChild.ChildString = str;

            Assert.AreEqual(str, _mockChild.ChildString);
        }

        [Test]
        public void ChildStubSetsBasePropertiesCorrectly()
        {
            String str = "Child's base stub";

            _mockChild.BaseString = str;

            Assert.AreEqual(str, _mockChild.BaseString);
        }

        #endregion

    }
}
