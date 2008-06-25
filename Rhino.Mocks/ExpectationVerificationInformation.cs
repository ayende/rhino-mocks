using System.Collections.Generic;
using Rhino.Mocks.Generated;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks
{
	internal class ExpectationVerificationInformation
	{
	    private IExpectation expected;
		private IList<object[]> argumentsForAllCalls;
		private ExpectationsList expectationsToVerify;
		
		public IExpectation Expected { get { return expected; } set { expected = value; } }
		public IList<object[]> ArgumentsForAllCalls { get { return argumentsForAllCalls; } set { argumentsForAllCalls = value; }  }
		public ExpectationsList ExpectationsToVerify { get { return expectationsToVerify; } set { expectationsToVerify = value; }  }
	}
}
