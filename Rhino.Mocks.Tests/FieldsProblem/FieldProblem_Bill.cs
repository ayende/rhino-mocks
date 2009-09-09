using MbUnit.Framework;
using Microsoft.Practices.Unity;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Bill
    {
        /// <summary>
        /// From thread:
        /// http://groups.google.com/group/rhinomocks/browse_thread/thread/a22b18618be887ff?hl=en
        /// </summary>
        [Test]
        public void Should_be_able_to_proxy_IUnityContainer()
        {
            var unity = MockRepository.GenerateMock<IUnityContainer>();
        }
    }
}
