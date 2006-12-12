using System;
using System.Text;
using NUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    [TestFixture]
    public class FieldProblem_Daniel
    {
        public class ClassThatOverrideEquals
        {
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

        	public override int GetHashCode()
        	{
        		return base.GetHashCode();
        	}
        }

        [Test]
        public void MockClassWithEquals()
        {
            MockRepository mocks = new MockRepository();
            ClassThatOverrideEquals c = (ClassThatOverrideEquals)mocks.CreateMock(typeof(ClassThatOverrideEquals));
            c.Equals(c);
            LastCall.Return(false);
            mocks.Replay(c);
            Assert.IsFalse(c.Equals(c));
            mocks.Verify(c);
        }
    }
}
