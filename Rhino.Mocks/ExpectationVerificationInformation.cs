using System.Collections.Generic;
using Rhino.Mocks.Generated;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks
{
	public class ExpectationVerificationInformation
	{
		public IExpectation Expected { get; set; }

		public IList<object[]> ArgumentsForAllCalls { get; set; }

		public ExpectationsList ExpectationsToVerify { get; set; }
	}
}