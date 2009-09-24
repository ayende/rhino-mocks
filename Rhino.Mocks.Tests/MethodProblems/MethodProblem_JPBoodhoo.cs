using MbUnit.Framework;

namespace Rhino.Mocks.Tests.MethodProblems
{
    public class MethodProblem_JPBoodhoo
    {
        public interface InterfaceWithAMethodThatHasANameThatShouldNotBeRecognizedAsAnEvent
        {
            void add_MethodThatShouldNotBeSeenAsAnEvent(object item);
        }

        public class GenericClass
        {
            InterfaceWithAMethodThatHasANameThatShouldNotBeRecognizedAsAnEvent dependency;

            public GenericClass(InterfaceWithAMethodThatHasANameThatShouldNotBeRecognizedAsAnEvent dependency)
            {
                this.dependency = dependency;
            }

            public void do_something(object item)
            {
                dependency.add_MethodThatShouldNotBeSeenAsAnEvent(item);
            }
        }

        [TestFixture]
        public class when_stubbing_a_call_to_a_method_that_matches_the_naming_prefix_for_an_event_but_is_not_an_event
        {
            InterfaceWithAMethodThatHasANameThatShouldNotBeRecognizedAsAnEvent dependency;
            GenericClass system_under_test;
            object item;


            [SetUp]
            public void setup()
            {
                item = new object();
                dependency = MockRepository.GenerateStub<InterfaceWithAMethodThatHasANameThatShouldNotBeRecognizedAsAnEvent>();
                system_under_test = new GenericClass(dependency);
                system_under_test.do_something(item);
            }

            [Test]
            public void it_should_not_try_to_treat_it_as_an_event()
            {
                dependency.AssertWasCalled(generic_parameter => generic_parameter.add_MethodThatShouldNotBeSeenAsAnEvent(item));
            }
        }
    }
}