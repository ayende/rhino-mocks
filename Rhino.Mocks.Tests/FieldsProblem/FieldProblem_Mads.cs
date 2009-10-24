using System.Collections.Generic;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	
	public class FieldProblem_Mads
	{
		[Fact(Skip = @"YUCK! Unsolvable framework bug.
See here for the details:
http://groups.google.co.il/group/castle-project-devel/browse_thread/thread/1697e02d96c9c2df/d4e3e24f444ac712?lnk=st&q=Generic+interface+with+generic+method+with+constrained+on+generic+method+param+&rnum=1#d4e3e24f444ac712")]
		public void Unresolable_Framework_Bug_With_Generic_Method_On_Generic_Interface_With_Conditions_On_Both_Generics()
		{
			MockRepository mocks = new MockRepository();

			TestInterface<List<string>> mockedInterface =
				mocks.StrictMock<TestInterface<List<string>>>();
		}
	}

	public interface TestInterface<T> where T : IEnumerable<string>
	{
		string TestMethod<T2>(T2 obj) where T2 : T,
		                              	ICollection<string>;
	}
}