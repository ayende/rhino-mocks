#if DOTNET35
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{

	[TestFixture]
	public class FieldProblem_Benjamin
	{
#if DOTNET35 
		[Test]
        public void ThisTestPasses()
        {
            var interfaceStub = MockRepository.GenerateStub<InterfaceINeedToStub>();

            interfaceStub.Stub(x => x.MyStringValue).Return("string");
            interfaceStub.MyIntValue = 4;

            Assert.AreEqual(4, interfaceStub.MyIntValue);
        }

        [Test]
        public void ThisTestDoesNotPass()
        {
            var myInterface = MockRepository.GenerateStub<InterfaceINeedToStub>();

            // Changed order of property initialization
            myInterface.MyIntValue = 4;
            myInterface.Stub(x => x.MyStringValue).Return("string");

            Assert.AreEqual(4, myInterface.MyIntValue);
        }
#endif
	}

	public interface InterfaceINeedToStub
	{
		int MyIntValue { get; set; }
		string MyStringValue { get; }
	}
}
#endif