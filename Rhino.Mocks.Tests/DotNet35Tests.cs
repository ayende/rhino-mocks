#if DOTNET35
using Xunit;

namespace Rhino.Mocks.Tests
{
	using System;
	using Exceptions;

	
	public class DotNet35Tests
	{
		[Fact]
		public void NaturalSyntaxForCallingMethods()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.StrictMock<IDemo>();
			using (mocks.Record())
			{
				Expect.Call(demo.VoidNoArgs);
			}

			using (mocks.Playback())
			{
				demo.VoidNoArgs();
			}
		}

		[Fact]
		public void NaturalSyntaxForCallingMethods_WithArguments()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.StrictMock<IDemo>();
			using (mocks.Record())
			{
				Expect.Call( () => demo.VoidStringArg("blah") );
			}

			using (mocks.Playback())
			{
				demo.VoidStringArg("blah");
			}
		}

		[Fact]
		public void NaturalSyntaxForCallingMethods_WithArguments_WhenNotCalled_WouldFailVerification()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.StrictMock<IDemo>();
			using (mocks.Record())
			{
				Expect.Call(() => demo.VoidStringArg("blah"));
			}

			Throws.Exception<ExpectationViolationException>("IDemo.VoidStringArg(\"blah\"); Expected #1, Actual #0.",delegate
			{
				mocks.VerifyAll();
			});
		}

		[Fact]
		public void NaturalSyntaxForCallingMethods_WithArguments_WhenCalledWithDifferentArgument()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.StrictMock<IDemo>();
			using (mocks.Record())
			{
				Expect.Call(() => demo.VoidStringArg("blah"));
			}

			Throws.Exception<ExpectationViolationException>(@"IDemo.VoidStringArg(""arg""); Expected #0, Actual #1.
IDemo.VoidStringArg(""blah""); Expected #1, Actual #0.",delegate
			{
				demo.VoidStringArg("arg");
			});
		}

		[Fact]
		public void CanCallMethodWithParameters_WithoutSpecifyingParameters_WillAcceptAnyParameter()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.StrictMock<IDemo>();
			using (mocks.Record())
			{
				Expect.Call(() => demo.VoidStringArg("blah")).IgnoreArguments();
			}


			using (mocks.Playback())
			{
				demo.VoidStringArg("asd");
			}
		}
	}
}
#endif