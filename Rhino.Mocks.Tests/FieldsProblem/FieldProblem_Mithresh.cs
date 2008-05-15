using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	using MbUnit.Framework;

	[TestFixture]
	public class FieldProblem_Mithresh
	{
		[Test]
		public void TestOutMethod()
		{

			MockRepository mocks = new MockRepository();

			ITest mockProxy = mocks.StrictMock<ITest>();

			int intTest = 0;

			using (mocks.Record())
			{
				Expect.Call(delegate { mockProxy.Addnumber(out intTest);  }).OutRef(4);
			}
			using(mocks.Playback())
			{
				mockProxy.Addnumber(out intTest);
				Assert.AreEqual(4, intTest);
			}

		}

		public interface ITest
		{
			void Addnumber(out int Num);
		}
	}
}
