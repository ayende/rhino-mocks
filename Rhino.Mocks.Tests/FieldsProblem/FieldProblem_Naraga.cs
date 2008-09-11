#if DOTNET35
namespace Rhino.Mocks.Tests.FieldsProblem
{
	using System;
	using System.Threading;
	using MbUnit.Framework;

	[TestFixture]
	public class FieldProblem_Naraga
	{
		public interface IService
		{
			void Do(string msg);
		}

		[Test]
		public void MultiThreadedReplay()
		{
			var mocks = new MockRepository();
			var service = mocks.StrictMock<IService>();
			using (mocks.Record())
			{
				for (int i = 0; i < 100; i++)
				{
					int i1 = i;

					Expect.Call(() => service.Do("message" + i1));
				}
			}
			using (mocks.Playback())
			{
				int counter = 0;
				for (int i = 0; i < 100; i++)
				{
					var i1 = i;
					ThreadPool.QueueUserWorkItem(delegate
					{
						service.Do("message" + i1);
						Interlocked.Increment(ref counter);
					});
				}
				while (counter != 100)
					Thread.Sleep(100);
			}
		}
	}
}
#endif