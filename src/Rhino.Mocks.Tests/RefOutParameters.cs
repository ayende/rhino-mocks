using System;
using System.Text;
using NUnit.Framework;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class RefOutParameters
    {
        public class MyClass
        {
            public virtual void MyMethod(out int i, ref string s, int i1, out string s2)
            {
                throw new NotImplementedException(); 
            }
        }

        [Test]
        public void UseTheOutMethodToSpecifyOutputAndRefParameters()
        {
            MockRepository mocks = new MockRepository();
            MyClass myClass = (MyClass) mocks.CreateMock(typeof (MyClass));
            int i;
            string s = null, s2;
            myClass.MyMethod(out i, ref s, 1, out s2);
            LastCall.IgnoreArguments().OutRef(100, "s", "b");
            mocks.ReplayAll();
            
            myClass.MyMethod(out i, ref s, 1, out s2);
            
            mocks.VerifyAll();
            
            Assert.AreEqual(100, i);
            Assert.AreEqual("s", s);
            Assert.AreEqual("b", s2);
            
            
        }
    }
}
