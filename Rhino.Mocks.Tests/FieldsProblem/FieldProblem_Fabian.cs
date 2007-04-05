using MbUnit.Framework;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Fabian
	{
		public delegate R Func<A1, R>(A1 a1);

		public delegate void Proc<A1, A2>(A1 a1, A2 a2);

		public delegate int StringInt(string s);

		public interface ICache<TKey, TValue>
		{
			TValue GetValue(TKey key);
			void Add(TKey key, TValue value);
		}

		[Test]
		public void TestExpectCall()
		{
			MockRepository mocks = new MockRepository();
			ICache<string, int> mockCache = mocks.CreateMock<ICache<string, int>>();
			Expect.Call(mockCache.GetValue("a")).Do((Func<string, int>)delegate
			{
				return 1;
			});
			mocks.ReplayAll();

			int i = mockCache.GetValue("a");
			Assert.AreEqual(1,i );

			mocks.VerifyAll();
		}

		[Test]
		public void TestLastCall()
		{
			MockRepository mocks = new MockRepository();
			ICache<string, int> mockCache = mocks.CreateMock<ICache<string, int>>();
			mockCache.Add("a", 1);
			LastCall.Do((Proc<string, int>)delegate
			{
			});
			mocks.ReplayAll();

			mockCache.Add("a", 1);

			mocks.VerifyAll();
		}

		[Test]
		public void TestExpectCallWithNonGenericDelegate()
		{
			MockRepository mocks = new MockRepository();
			ICache<string, int> mockCache = mocks.CreateMock<ICache<string, int>>();
			IMethodOptions opts = Expect.Call(mockCache.GetValue("a"));
			opts.Do(new StringInt(GetValue));
			mocks.ReplayAll();

			int i = mockCache.GetValue("a");

			Assert.AreEqual(2, i);

			mocks.VerifyAll();
		}

		private int GetValue(string s)
		{
			return 2;
		}
	}
}