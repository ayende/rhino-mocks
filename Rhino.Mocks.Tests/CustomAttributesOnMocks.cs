using System;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class CustomAttributesOnMocks
    {
        [Test]
        public void Mock_will_have_Protect_attriute_defined_on_them()
        {
            var disposable = MockRepository.GenerateMock<IDisposable>();
            Assert.IsTrue(disposable.GetType().IsDefined(typeof (__ProtectAttribute), true));
        }
    }
}