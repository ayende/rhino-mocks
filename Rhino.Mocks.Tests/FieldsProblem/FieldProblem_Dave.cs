using System;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Dave
    {
        [Test]
        public void CallingMethodTwiceAndAssertingWasCalledOnce()
        {
            // IFormatProvider was the first thing that popped up, the type really doesn't matter here
            var mock = MockRepository.GenerateMock<IFormatProvider>();

            // two calls to the same method
            mock.GetFormat(typeof(string));
            mock.GetFormat(typeof(string));

            // this throws in RTM, did not in the RC
            // also tried Repeat.Any but that throws a different exception
            // saying that it is not supported on AssertWasCalled
            mock.AssertWasCalled(x => x.GetFormat(typeof(string)));
        }

        [Test]
        public void CallingMethodTwiceAndAssertingWasCalledTwice()
        {
            // IFormatProvider was the first thing that popped up, the type really doesn't matter here
            var mock = MockRepository.GenerateMock<IFormatProvider>();

            // two calls to the same method
            mock.GetFormat(typeof(string));
            mock.GetFormat(typeof(string));

            // this throws in RTM, did not in the RC
            // also tried Repeat.Any but that throws a different exception
            // saying that it is not supported on AssertWasCalled
            mock.AssertWasCalled(x => x.GetFormat(typeof(string)), o=>o.Repeat.Twice());
        }
    }
}