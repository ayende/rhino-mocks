using System.Collections.Generic;
using MbUnit.Framework;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	[Ignore(@"YUCK! Unsolvable framework bug.
See here for the details:
http://groups.google.co.il/group/castle-project-devel/browse_thread/thread/1697e02d96c9c2df/d4e3e24f444ac712?lnk=st&q=Generic+interface+with+generic+method+with+constrained+on+generic+method+param+&rnum=1#d4e3e24f444ac712")]
	public class FieldProblem_Mads
	{
		[Test]
		public void TestMethodTest()
		{
			MockRepository mocks = new MockRepository();

			TestInterface<List<string>> mockedInterface =
				mocks.CreateMock<TestInterface<List<string>>>();
		}
	}

	public interface TestInterface<T> where T : IEnumerable<string>
	{
		string TestMethod<T2>(T2 obj) where T2 : T,
		                              	ICollection<string>;
	}
}