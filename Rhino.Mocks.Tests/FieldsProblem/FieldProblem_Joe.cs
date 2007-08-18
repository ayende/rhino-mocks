using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Joe
	{
		[Test]
		public void MockingConcreteForm()
		{
			MockRepository mocks = new MockRepository();
			Form frm = mocks.PartialMock<Form>();
			Assert.IsNotNull(frm);
		}
	}
}
