using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_OrenEl
	{
		[Test]
		public void GenericUnsignTypes()
		{
			MockRepository mocks = new MockRepository();
			IDictionary<ulong, ushort> stubClusterNodesMap = mocks.Stub<IDictionary<ulong, ushort>>();
		}

		[Test]
		public void IndexedProperties()
		{
			IDictionary<ulong, ushort> stubClusterNodesMap = MockRepository.GenerateStub<IDictionary<ulong, ushort>>();
			stubClusterNodesMap[UInt64.MaxValue] = UInt16.MinValue;
			Assert.AreEqual(UInt16.MinValue, stubClusterNodesMap[UInt64.MaxValue]);
		}
	}
}
