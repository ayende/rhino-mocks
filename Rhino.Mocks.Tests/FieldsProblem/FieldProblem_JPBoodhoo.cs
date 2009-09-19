using System;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public class FieldProblem_JPBoodhoo
    {
        public class VirtualClass
        {
            public virtual DateTime virtual_property_public_read_private_write { get; private set; }
            public virtual string run_sheet_name { get; set; }
        }

        [TestFixture]
        public class when_setting_up_a_return_value_for_a_virtual_property_on_a_class_with_a_public_getter_and_private_setter
        {
            VirtualClass target;

            [SetUp]
            public void setup()
            {
                target = MockRepository.GenerateStub<VirtualClass>();
                target.Stub(entry_model => entry_model.virtual_property_public_read_private_write).Return(DateTime.Now);
            }

            [Test]
            public void should_not_throw_the_exception_suggesting_to_assign_the_property_value_directly()
            {
                target.virtual_property_public_read_private_write.Equals(DateTime.Now);
            }
        }
    }
}