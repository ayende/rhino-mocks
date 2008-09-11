namespace Rhino.Mocks.Tests.FieldsProblem
{
	using MbUnit.Framework;

	[TestFixture]
	public class GenericMethodWithOutDecimalParameterTest
	{
		public interface IMyInterface
		{
			void GenericMethod<T>(out T parameter);
		}

		[Test]
		public void GenericMethodWithOutDecimalParameter()
		{
			MockRepository mocks = new MockRepository();
			IMyInterface mock = mocks.StrictMock<IMyInterface>();

			decimal expectedOutParameter = 1.234M;
			using (mocks.Record())
			{
				decimal emptyOutParameter;
				mock.GenericMethod(out emptyOutParameter);
				LastCall.OutRef(expectedOutParameter);
			}

			using (mocks.Playback())
			{
				decimal outParameter;
				mock.GenericMethod(out outParameter);
				Assert.AreEqual(expectedOutParameter, outParameter);
			}
		}

		public static void Foo(out decimal d)
		{
			d = 1.234M;
		}

		public static void Foo(out int d)
		{
			d = 1;
		}
	}
}