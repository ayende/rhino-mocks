using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace RhinoMocksSetupResult
{
    /// <summary>
    /// Sometimes you have a method on your mocked object
    /// for which you don't care how or if it was called, 
    /// but you may want to set a return value (or an exception to be thrown)
    /// in case it is called
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+Setup+Result.ashx"/>
    [TestFixture]
    public class RhinoMocksSetupResultTest
    {
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void SetupResultUsingOrdered()
        {
           MockRepository mocks = new MockRepository();
           IDemo demo = mocks.StrictMock<IDemo>();
           
           SetupResult.For(demo.Prop).Return("Ayende");

           using(mocks.Ordered())
           {
               demo.VoidNoArgs();
               LastCall.On(demo).Repeat.Twice();
           }
           mocks.ReplayAll();

           demo.VoidNoArgs();
           for (int i = 0; i < 30; i++)
           {
               Assert.AreEqual("Ayende",demo.Prop);      
           }
           demo.VoidNoArgs();

           mocks.VerifyAll();
        }
    }
}
