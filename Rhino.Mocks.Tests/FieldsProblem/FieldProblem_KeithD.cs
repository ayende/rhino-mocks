namespace Rhino.Mocks.Tests.FieldsProblem
{
    using System;
    using Rhino.Mocks.Interfaces;
    using Xunit;

    public class FieldProblem_KeithD
    {
        [Fact]
        public void ImplementedTypesOfTargetOfMockedDelegateDoesNotContainDelegateType()
        {
            var mockDelegate = MockRepository.GenerateMock<EventHandler>();

            var mockTarget = mockDelegate.Target as IMockedObject;

            Assert.NotNull(mockTarget);
            Assert.DoesNotContain(typeof(EventHandler), mockTarget.ImplementedTypes);
        }

        [Fact]
        public void CanStubReturningAnonymousDelegateTwice()
        {
            var foo = MockRepository.GenerateMock<IFoo>();

            foo.Stub(x => x.Bar(0)).Return(() => 2);
            foo.Stub(x => x.Bar(1)).Return(() => 4);

            Assert.Equal(2, foo.Bar(0)());
            Assert.Equal(4, foo.Bar(1)());
        }

        [Fact]
        public void CanStubReturningNonAnonymousDelegateTwice()
        {
            var foo = MockRepository.GenerateMock<IFoo>();

            foo.Stub(x => x.Bar(0)).Return(Return2);
            foo.Stub(x => x.Bar(1)).Return(Return4);

            Assert.Equal(2, foo.Bar(0)());
            Assert.Equal(4, foo.Bar(1)());
        }

        public interface IFoo
        {
            Func<int> Bar(int baz);
        }

        private static int Return2() { return 2; }

        private static int Return4() { return 4; }
    }
}
