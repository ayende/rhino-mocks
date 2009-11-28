using Xunit;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
	
	public class PropertySetterFixture
	{
		[Fact]
		public void Setter_Expectation_With_Custom_Ignore_Arguments()
		{
			MockRepository mocks = new MockRepository();

			IBar bar = mocks.StrictMock<IBar>();

			using(mocks.Record())
			{
				Expect.Call(bar.Foo).SetPropertyAndIgnoreArgument();
			}

			using(mocks.Playback())
			{
				bar.Foo = 2;
			}

			mocks.VerifyAll();
		}

		[Fact]
		public void Setter_Expectation_Not_Fullfilled()
		{
			MockRepository mocks = new MockRepository();

			IBar bar = mocks.StrictMock<IBar>();

			using (mocks.Record())
			{
				Expect.Call(bar.Foo).SetPropertyAndIgnoreArgument();
			}

			Assert.Throws<ExpectationViolationException>("IBar.set_Foo(any); Expected #1, Actual #0.", () =>
			{
				using (mocks.Playback())
				{
				}
			});
		}

		[Fact]
		public void Setter_Expectation_With_Correct_Argument()
		{
			MockRepository mocks = new MockRepository();

			IBar bar = mocks.StrictMock<IBar>();

			using (mocks.Record())
			{
				Expect.Call(bar.Foo).SetPropertyWithArgument(1);
			}

			using (mocks.Playback())
			{
				bar.Foo = 1;
			}

			mocks.VerifyAll();
		}

		[Fact]
		public void Setter_Expectation_With_Wrong_Argument()
		{
			MockRepository mocks = new MockRepository();

			IBar bar = mocks.StrictMock<IBar>();

			using (mocks.Record())
			{
				Expect.Call(bar.Foo).SetPropertyWithArgument(1);
			}

			mocks.Playback();
			Assert.Throws<ExpectationViolationException>(
				"IBar.set_Foo(0); Expected #0, Actual #1.\r\nIBar.set_Foo(1); Expected #1, Actual #0.", () => { bar.Foo = 0; });
		}
	}

	public interface IBar
	{
		int Foo { get; set; }
	}
}