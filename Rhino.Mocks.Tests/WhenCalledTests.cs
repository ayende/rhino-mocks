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
			var wasCalled = false;
			var stub = MockRepository.GenerateStub<IDemo>();
			stub.Stub(x => x.StringArgString(Arg.Is("")))
				.Return("blah")
				.WhenCalled(delegate { wasCalled = true; });
			Assert.AreEqual("blah", stub.StringArgString(""));
			Assert.IsTrue(wasCalled);
		}

		[Test]
		public void Can_modify_return_value()
		{
			var stub = MockRepository.GenerateStub<IDemo>();
			stub.Stub(x => x.StringArgString(Arg.Is("")))
				.Return("blah")
				.WhenCalled(invocation => invocation.ReturnValue = "arg");
			Assert.AreEqual("arg", stub.StringArgString(""));
		}

		[Test]
		public void Can_inspect_method_arguments()
		{
			var stub = MockRepository.GenerateStub<IDemo>();
			stub.Stub(x => x.StringArgString(null))
				.IgnoreArguments()
				.Return("blah")
				.WhenCalled(invocation => Assert.AreEqual("foo", invocation.Arguments[0]));
			Assert.AreEqual("blah", stub.StringArgString("foo"));
		}

	}
}
#endif