using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	
	public class FieldProblem_OrenEl
	{
		[Fact]
		public void GenericUnsignTypes()
		{
			MockRepository mocks = new MockRepository();
			IDictionary<ulong, ushort> stubClusterNodesMap = mocks.Stub<IDictionary<ulong, ushort>>();
		}

		[Fact]
		public void IndexedProperties()
		{
			IDictionary<ulong, ushort> stubClusterNodesMap = MockRepository.GenerateStub<IDictionary<ulong, ushort>>();
			stubClusterNodesMap[UInt64.MaxValue] = UInt16.MinValue;
			Assert.Equal(UInt16.MinValue, stubClusterNodesMap[UInt64.MaxValue]);
		}
	}
}
