using System;
using System.Collections.Generic;
using System.Text;
using ADODB;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_dyowee
	{
		[Test]
		public void MockingRecordSet()
		{
			MockRepository mr = new MockRepository();
			Recordset mock = mr.StrictMock<ADODB.Recordset>();
			Assert.IsNotNull(mock);
			Expect.Call(mock.ActiveConnection).Return("test");
			mr.ReplayAll();
			Assert.AreEqual("test", mock.ActiveConnection);
			mr.VerifyAll();
		}
	}
}
