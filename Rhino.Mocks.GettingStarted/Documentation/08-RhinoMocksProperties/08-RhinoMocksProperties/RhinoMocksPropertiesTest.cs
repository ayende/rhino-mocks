using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections;

namespace RhinoMocksProperties
{
    /// <summary>
    /// If you want to mock properties, 
    /// you need to keep in mind that properties are merely special syntax for normal methods,
    /// a get property will translate directly to propertyType get_PropertyName()
    /// and a set property will translate directory to void set_PropertyName(propertyType value).
    /// 
    /// So how do you create expectations for a property? Exactly as you would for those methods.
    /// </summary>
    /// <see cref="http://www.ayende.com/wiki/Rhino+Mocks+Properties.ashx"/>
    [TestFixture]
    public class RhinoMocksPropertiesTest
    {
        /// <summary>
        /// Here is how you set the return value for a get property:
        /// </summary>
        [Test]
        public void setPropertyReturnValue()
        {
            MockRepository mocks = new MockRepository();
            IList list = mocks.StrictMock<IList>();
            SetupResult.For(list.Count).Return(42);

            mocks.ReplayAll();

            Assert.AreEqual(42, list.Count);

            mocks.VerifyAll();
        }

        /// <summary>
        /// This test doesnt compile. 
        /// CreateType method is unknow and any resource about it is available on the net.
        /// Perhaps, this method isn't support anymore
        /// </summary>
        [Test]
        public void setPropertyExpectation()
        {
            MockRepository mocks = new MockRepository();
            ArrayList list = mocks.StrictMock<ArrayList>();//Capacity property doesn't exist in IList
            list.Capacity = 500;//Will create an expectation for this call
            LastCall.IgnoreArguments();//Ignore the amount that is passed.
        }

        [Test]
        public void PropertyBehaviorForSingleProperty()
        {
            
            MockRepository mocks = new MockRepository();
            IDemo demo = mocks.StrictMock<IDemo>();

            Expect.Call(demo.Prop).PropertyBehavior();

            mocks.ReplayAll();

            for (int i = 0; i < 49; i++)
            {
                demo.Prop = "ayende" + i;
                Assert.AreEqual("ayende" + i, demo.Prop);
            }

            mocks.VerifyAll();
        }
    }
}
