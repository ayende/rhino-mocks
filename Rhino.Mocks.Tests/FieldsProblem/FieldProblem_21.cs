using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public class FieldProblem_21
    {
        public interface Foo
        {
            Action<int> Bar(); //Action<T>, T can be of any type
        }

        [Fact]
        public void TestMethod1()
        {
            var mock = MockRepository.GenerateMock<Foo>();
            mock.Expect(e => e.Bar()).Return(x => { }); //OK
            mock.Expect(e =>
            {
                var action = e.Bar();
                Console.WriteLine(action);
                return action;
            }).Return(x => { }); //Exception: System.InvalidCastException
        }

        [Fact]
        public void TestMethod2()
        {
            var mock = MockRepository.GenerateMock<Foo>();
            mock.Expect(e => e.Bar()).Repeat.Once().Return(x => { }); //OK
            mock.Expect(e => e.Bar()).Repeat.Once().Return(x => { }); //Exception: System.InvalidCastException
        }
    }
}