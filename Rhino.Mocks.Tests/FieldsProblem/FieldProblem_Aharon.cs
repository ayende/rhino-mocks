using System;
using System.Runtime.InteropServices;
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;

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

		[Test]
		[ExpectedException(typeof(ExpectationViolationException),"Accepter.Accept(Rhino.Mocks.Tests.FieldsProblem.Accepter); Expected #0, Actual #1.")]
		public void PassingMockToMock_WhenErrorOccurs()
		{
			MockRepository mocks = new MockRepository();
			Accepter accepter = mocks.CreateMock<Accepter>();
			mocks.ReplayAll();
			accepter.Accept(accepter);

		}
	}

	public abstract class Accepter
	{
		public abstract void Accept(Accepter demo);

		public override string ToString()
		{
			return base.ToString();
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