#if DOTNET35
using MbUnit.Framework;


namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class WhenCalledTests
	{
		[Test]
		public void Shortcut_to_arg_is_equal()
		{
			// minor hack to get this to work reliably, we reset the arg manager,
			// and restore on in the MockRepository ctor, so we do it this way
			new MockRepository();
			Assert.AreEqual(Arg.Is(1), Arg<int>.Is.Equal(1));
		}

		[Test]
		public void Can_use_when_called_to_exceute_code_when_exceptation_is_matched_without_stupid_delegate_sig_overhead()
		{
			var wasCalled = true;
			var stub = MockRepository.GenerateStub<IDemo>();
			stub.Stub(x => x.StringArgString(Arg.Is("")))
				.Return("blah")
				.Do(delegate { wasCalled = true; });
			Assert.AreEqual("blah", stub.StringArgString(""));
			Assert.IsTrue(wasCalled);
		}

		[Test]
		public void Can_modify_return_value()
		{
			var wasCalled = true;
			var stub = MockRepository.GenerateStub<IDemo>();
			stub.Stub(x => x.StringArgString(Arg.Is("")))
				.Return("blah")
				.Do(invocation => invocation.ReturnValue = "arg");
			Assert.AreEqual("arg", stub.StringArgString(""));
			Assert.IsTrue(wasCalled);
		}
	}
}
#endif