using System;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class MockWithRefAndOutParams
    {
        MockRepository mocks;
        IRefAndOut target;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            target = (IRefAndOut)mocks.CreateMock(typeof(IRefAndOut));
        }

        [Test]
        public void RefString()
        {
            string s = "";
            target.RefStr(ref s);
            LastCall.Do(new RefStrDel(SayHello));
            mocks.ReplayAll();
            target.RefStr(ref s);
            Assert.AreEqual("Hello", s);
        }

        [Test]
        public void OutString()
        {
            string s = "";
            target.OutStr(out s);
            LastCall.Do(new OutStrDel(OutSayHello));
            mocks.ReplayAll();
            target.OutStr(out s);
            Assert.AreEqual("Hello", s);
        }

        [Test]
        public void OutInt()
        {
            int i = 0;
            target.OutInt(out i);
            LastCall.Do(new OutIntDel(OutFive));
            mocks.ReplayAll();
            target.OutInt(out i);
            Assert.AreEqual(5, i);
        }
        
        [Test]
        public void RefInt()
        {
            int i = 0;
            target.RefInt(ref i);
            LastCall.Do(new RefIntDel(RefFive));
            mocks.ReplayAll();
            target.RefInt(ref i);
            Assert.AreEqual(5, i);
        }

        private void RefFive(ref int i)
        {
            i = 5;
        }

        private void SayHello(ref string s)
        {
            s = "Hello";
        }

        private void OutFive(out int i)
        {
            i = 5;
        }

        private void OutSayHello(out string s)
        {
            s = "Hello";
        }
        
        public delegate void RefStrDel(ref string s);
        public delegate void RefIntDel(ref int i);
        public delegate void OutStrDel(out string s);
        public delegate void OutIntDel(out int i);
    
    }

    public interface IRefAndOut
    {
        void RefInt(ref int i);
        void RefStr(ref string s);

        void OutStr(out string s);
        void OutInt(out int i);
    }
}
