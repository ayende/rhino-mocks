using System;
using System.Text;
using NUnit.Framework;
using System.Web.UI;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_David
    {
        [Test]
        public void MockWebUIPageClass()
        {
            MockRepository mocks = new MockRepository();
            Page page = (Page)mocks.CreateMock(typeof(Page));
            page.Validate();
            mocks.ReplayAll();
            page.Validate();
            mocks.VerifyAll();
        }

        [Test]
        public void MockClassWithVirtualMethodCallFromConstructor()
        {
            MockRepository mocks = new MockRepository();
            ClassWithVirtualMethodCallFromConstructor cwvmcfc = (ClassWithVirtualMethodCallFromConstructor)mocks.CreateMock(typeof(ClassWithVirtualMethodCallFromConstructor));
            Assert.IsNotNull(cwvmcfc);
            Expect.Call(cwvmcfc.ToString()).Return("Success");
            mocks.ReplayAll();
            Assert.AreEqual("Success", cwvmcfc.ToString());
            mocks.VerifyAll();
        }

        public class ClassWithVirtualMethodCallFromConstructor
        {
            public ClassWithVirtualMethodCallFromConstructor()
            {
                VirtualCall();
            }

            public override string ToString()
            {
                return base.ToString();
            }

            public virtual void VirtualCall() { }
        }
    }
}
