using System;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks.Impl;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Bruce
    {
        [Test]
        public void CreateClassWithDefaultCtor()
        {
            MockRepository mocks = new MockRepository();
            ClassWithDefaultCtor cwdc = (ClassWithDefaultCtor)mocks.DynamicMock(typeof(ClassWithDefaultCtor));
            Assert.IsNotNull(cwdc);
        }

        [Test]
        public void HandlingArraysWithValueTypeArrays()
        {
            Assert.IsTrue(Validate.ArgsEqual(new object[] { new ushort[0] }, new object[] { new ushort[0] }));
        }

        public class ClassWithDefaultCtor
        {
            public ClassWithDefaultCtor()
            {}
        }
    }
}
