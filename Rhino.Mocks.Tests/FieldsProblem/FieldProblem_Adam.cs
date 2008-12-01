#if DOTNET35
using System;
using System.ComponentModel;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Adam
    {
        public interface IFoo
        {
            string Str { get; set; }
            event EventHandler Event;
        }


        [Test]
        public void ShouldRaiseEventWhenEverPropIsSet()
        {
            var foo = MockRepository.GenerateMock<IFoo>();
            foo.Stub(x => x.Str = Arg<string>.Is.Anything)
                .WhenCalled(x => foo.Raise(y => y.Event += null, foo, EventArgs.Empty));

            int calls = 0;
            foo.Event += delegate
            {
                calls += 1;
            };

            foo.Str = "1";
            foo.Str = "2";
            foo.Str = "3";
            foo.Str = "4";

            Assert.AreEqual(4, calls);
        }
    }
}
#endif