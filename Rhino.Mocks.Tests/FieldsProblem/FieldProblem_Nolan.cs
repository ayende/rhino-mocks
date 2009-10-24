using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

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

    
    public class StubDemoTestFixture : IDisposable
    {

        #region Variables

        private MockRepository _mocks;
        private IBase _mockBase;
        private IChild _mockChild;

        #endregion

        #region Setup and Teardown

		public StubDemoTestFixture()
        {
            _mocks = new MockRepository();

            _mockBase = _mocks.Stub<IBase>();
            _mockChild = _mocks.Stub<IChild>();

            _mocks.ReplayAll();
        }

        public void Dispose()
        {
            _mocks.VerifyAll();
        }

        #endregion

        #region Tests

        [Fact]
        public void BaseStubSetsBasePropertiesCorrectly()
        {
            String str = "Base stub";

            _mockBase.BaseString = str;

            Assert.Equal(str, _mockBase.BaseString);
        }

        [Fact]
        public void ChildStubSetsChildPropertiesCorrectly()
        {
            String str = "Child stub";

            _mockChild.ChildString = str;

            Assert.Equal(str, _mockChild.ChildString);
        }

        [Fact]
        public void ChildStubSetsBasePropertiesCorrectly()
        {
            String str = "Child's base stub";

            _mockChild.BaseString = str;

            Assert.Equal(str, _mockChild.BaseString);
        }

        #endregion

    }
}
