using System;
using System.Text;
using System.Windows.Forms;
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class PartialMockTests
    {
        MockRepository mocks;
        AbstractClass abs;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            abs = (AbstractClass)mocks.PartialMock(typeof(AbstractClass));
        }

        [Test]
		public void AutomaticallCallBaseMethodIfNoExpectationWasSet() 
	    {
            mocks.ReplayAll();
            Assert.AreEqual(1, abs.Increment());
			Assert.AreEqual(6, abs.Add(5));
			Assert.AreEqual(6, abs.Count);
			mocks.VerifyAll();

	    }

        [Test]
        public void CanMockVirtualMethods()
        {
            Expect.Call(abs.Increment()).Return(5);
			Expect.Call(abs.Add(2)).Return(3);
			mocks.ReplayAll();
            Assert.AreEqual(5, abs.Increment());
			Assert.AreEqual(3, abs.Add(2));
			Assert.AreEqual(0, abs.Count);
            mocks.VerifyAll();
        }

        [Test]
        public void CanMockAbstractMethods()
        {
            Expect.Call(abs.Decrement()).Return(5);
            mocks.ReplayAll();
            Assert.AreEqual(5, abs.Decrement());
            Assert.AreEqual(0, abs.Count);
            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Can't create a partial mock from an interface")]
        public void CantCreatePartialMockFromInterfaces()
        {
            new MockRepository().PartialMock(typeof(IDemo));
        }

        [Test]
        [ExpectedException(typeof(ExpectationViolationException), "AbstractClass.Decrement(); Expected #0, Actual #1.")]
        public void CallAnAbstractMethodWithoutSettingExpectation()
        {
            mocks.ReplayAll();
            abs.Decrement();
        }

    	[Test]
    	public void CanMockWithCtorParams()
    	{
    		WithParameters withParameters = mocks.PartialMock<WithParameters>(1);
    		Expect.Call(withParameters.Int).Return(4);
    		mocks.ReplayAll();
    		Assert.AreEqual(4, withParameters.Int);
    		mocks.VerifyAll();
    	}
    }
    
    public abstract class AbstractClass
    {
        public int Count = 0;

        public virtual int Increment()
        {
            return ++Count;
        }

        public virtual int Add(int n)
		{
			return Count += n;
		}

        public abstract int Decrement();
    }

	public class WithParameters
	{
		private int i;


		public WithParameters(int i)
		{
			this.i = i;
		}


		public virtual int Int
		{
			get { return i; }
			set { i = value; }
		}
	}
}
