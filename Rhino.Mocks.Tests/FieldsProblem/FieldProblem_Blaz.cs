using NUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Blaz
    {
        [Test]
        public void SameNameInterface()
        {
            MockRepository mocks = new MockRepository();
            IDemo demo1 = (IDemo)mocks.CreateMock(typeof(IDemo));
            Other.IDemo demo2 = (Other.IDemo)mocks.CreateMock(typeof(Other.IDemo));
            
            Assert.AreNotEqual(demo1.GetType(), demo2.GetType());
        }
    }
    namespace Other
    {
        public interface IDemo
        {
            string Name { get; set; }
        }
    }
}
