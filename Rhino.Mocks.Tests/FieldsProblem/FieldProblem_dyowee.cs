using System;
using System.Collections.Generic;
using System.Text;
using ADODB;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	
	public class FieldProblem_dyowee
	{
		[Fact]
		public void MockingRecordSet()
		{
			MockRepository mr = new MockRepository();
			Recordset mock = mr.StrictMock<ADODB.Recordset>();
			Assert.NotNull(mock);
			Expect.Call(mock.ActiveConnection).Return("test");
			mr.ReplayAll();
			Assert.Equal("test", mock.ActiveConnection);
			mr.VerifyAll();
		}
	}
}
