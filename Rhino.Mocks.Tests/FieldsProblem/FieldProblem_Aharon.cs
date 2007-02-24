using System;
using System.Runtime.InteropServices;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Aharon
	{
		[Test]
		public void CanCreateInterfaceWithGuid()
		{
			MockRepository mocks = new MockRepository();
			IUniqueID bridgeRemote = mocks.CreateMock<IUniqueID>();
			Assert.IsNotNull(bridgeRemote);
		}


		[Test]
		public void MockingDataset()
		{
			MockRepository mocks = new MockRepository();
			MyDataSet controller = mocks.CreateMock<MyDataSet>();
			Assert.IsNotNull(controller);
		}
	}

	[Guid("AFCF8240-61AF-4089-BD98-3CD4D719EDBA")]
	public interface IUniqueID
	{
	}


	public class MyDataSet : System.Data.DataSet, IClearable
	{
		
	}

	public interface IClearable
	{
		void Clear();
	}
}