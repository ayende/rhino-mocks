using MbUnit.Framework;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class IndexerTests
	{
		public interface IndexerInterface
		{
			string this[string idname] { get; }
			string this[int id] { get; }
		}

		[Test]
		public void SettingExpectationOnIndexer()
		{
			MockRepository mocks = new MockRepository();
			IndexerInterface indexer = (IndexerInterface) mocks.CreateMock(typeof (IndexerInterface));
			Expect.On(indexer).Call(indexer["1"]).Return("First");
			mocks.ReplayAll();
			Assert.AreEqual("First", indexer["1"]);
			mocks.VerifyAll();
		}
	}
}