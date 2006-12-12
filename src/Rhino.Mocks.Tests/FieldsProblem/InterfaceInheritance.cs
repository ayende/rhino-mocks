using System;
using NUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    //Bug 46
    [TestFixture]
    public class InterfaceInheritance
    {
        [Test]
        public void CreateObjectUsingInterfaceInheritance()
        {
            MockRepository mocks = new MockRepository();
            ILocalizer localizer = (ILocalizer)mocks.CreateMock(typeof(ILocalizer));
            Assert.IsNotNull(localizer);
            typeof(ILocalizer).IsAssignableFrom(localizer.GetType());
            typeof(ICloneable).IsAssignableFrom(localizer.GetType());
            mocks.ReplayAll();
            mocks.VerifyAll();
        }

        public interface ILocalizer : ICloneable
        {
            void SomeThing();
        }
    }
}