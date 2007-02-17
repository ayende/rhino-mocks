using System;
using System.Text;
using MbUnit.Framework;

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

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Output and ref parameters has already been set for this expectation")]
        public void UseTheOutMethodToSpecifyOutputAndRefParameters_CanOnlyBeCalledOnce()
        {
            MockRepository mocks = new MockRepository();
            MyClass myClass = (MyClass) mocks.CreateMock(typeof (MyClass));
            int i;
            string s = null, s2;
            myClass.MyMethod(out i, ref s, 1, out s2);
			LastCall.OutRef(100, "s", "b").OutRef(100, "s", "b");
        }

    	[Test]
    	public void GivingLessParametersThanWhatIsInTheMethodWillNotThrow()
    	{
    		   MockRepository mocks = new MockRepository();
            MyClass myClass = (MyClass) mocks.CreateMock(typeof (MyClass));
            int i;
            string s = null, s2;
            myClass.MyMethod(out i, ref s, 1, out s2);
            LastCall.IgnoreArguments().OutRef(100);
            mocks.ReplayAll();
            
            myClass.MyMethod(out i, ref s, 1, out s2);
            
            mocks.VerifyAll();
            
            Assert.AreEqual(100, i);
            Assert.IsNull(s);
            Assert.IsNull(s2);
    	}
    }
}
