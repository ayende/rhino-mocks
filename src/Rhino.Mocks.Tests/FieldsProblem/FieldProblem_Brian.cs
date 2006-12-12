#if dotNet2
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Brian
    {
        [Test]
        public void SetExpectationOnNullableValue()
        {
            MockRepository mocks = new MockRepository();
            IFoo foo = mocks.CreateMock<IFoo>();

            int? id = 2;

            Expect.Call(foo.Id).Return(id).Repeat.Twice();
            Expect.Call(foo.Id).Return(null);
            Expect.Call(foo.Id).Return(1);

            mocks.ReplayAll();

            Assert.IsTrue(foo.Id.HasValue);
            Assert.AreEqual(2, foo.Id.Value);
            Assert.IsFalse(foo.Id.HasValue);
            Assert.AreEqual(1, foo.Id.Value);

            mocks.VerifyAll();
        }

        public interface IFoo
        {
            int? Id { get;}
        }
    }
}
#endif