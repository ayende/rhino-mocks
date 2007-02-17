using System;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class ClassWithFinalizer
	{
		~ClassWithFinalizer()
		{
			
		}
	}
	[TestFixture]
	public class FieldProblem_Eric
	{
		[Test]
		public void MockAClassWithFinalizer()
		{
			MockRepository mocks = new MockRepository();
			ClassWithFinalizer withFinalizer = (ClassWithFinalizer) mocks.CreateMock(typeof (ClassWithFinalizer));
			mocks.ReplayAll();
			mocks.VerifyAll();//move it to verify state
			withFinalizer = null;// abandon the variable, will make it avialable for GC.
			GC.WaitForPendingFinalizers();
		}
	}
}
