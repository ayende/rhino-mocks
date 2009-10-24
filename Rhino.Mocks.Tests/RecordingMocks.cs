#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

#if DOTNET35
using System;
using System.Collections.Generic;
using Xunit;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
	
	public class RecordingMocks
	{
		[Fact]
		public void CanGetMockRepositoryForMock()
		{
			MockRepository repository = new MockRepository();
			var foo = repository.StrictMock<IFoo54>();
			Assert.Same(repository, foo.GetMockRepository());
		}

		[Fact]
		public void CanResetStubAndReuseIt()
		{
			var foo = MockRepository.GenerateStub<IFoo54>();

			foo.Stub(x => x.bar()).Return("open");
			// several calls to 'foo.bar()
			Assert.Equal(foo.bar(), "open");
			Assert.Equal(foo.bar(), "open");

			foo.BackToRecord();

			foo.Stub(x => x.bar()).Return("closed");

			foo.Replay();

			// several calls to 'foo.bar()
			Assert.Equal(foo.bar(), "closed");
			Assert.Equal(foo.bar(), "closed");
		}


		[Fact]
		public void WhenStubbingWillAllowManyCallsOnTheSameExpectation()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.DoSomething()).Return(1);

			for (int i = 0; i < 59; i++)
			{
				Assert.Equal(1, foo54.DoSomething());
			}
		}

		[Fact]
		public void WhenStubbingCanSetNumberOfCallsThatWillBeMatched()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.DoSomething()).Return(1).Repeat.Once();
			foo54.Stub(x => x.DoSomething()).Return(2).Repeat.Once();

			Assert.Equal(1, foo54.DoSomething());
			Assert.Equal(2, foo54.DoSomething());
			Assert.Equal(0, foo54.DoSomething());// unexpected
		}

		[Fact]
		public void CanAssertOnCountOfCallsMade()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.DoSomething()).Return(1);

			Assert.Equal(1, foo54.DoSomething());

			foo54.AssertWasCalled(x => x.DoSomething());
			Assert.Equal(1, foo54.GetArgumentsForCallsMadeOn(x => x.DoSomething()).Count);
		}

		[Fact]
		public void CanAssertMethodArgumentsNaturally()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.Bar("asd")).Return(1);

			Assert.Equal(1, foo54.Bar("asd"));

			foo54.AssertWasCalled(x => x.Bar(Arg.Text.EndsWith("d")));
		}

		[Fact]
		public void CanAssertMethodArgumentsNaturally_WhenFailed()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.Bar("asdg")).Return(1);

			Assert.Equal(1, foo54.Bar("asdg"));

			Assert.Throws<ExpectationViolationException>(
				"IFoo54.Bar(ends with \"d\"); Expected #1, Actual #0.",
				() => foo54.AssertWasCalled(x => x.Bar(Arg.Text.EndsWith("d"))));
		}

		[Fact]
		public void WhenCallingMethodWithNoParameters_WillReturnZeroLengthArrayForEachCall()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.DoSomething()).Return(1);

			Assert.Equal(1, foo54.DoSomething());

			foo54.AssertWasCalled(x => x.DoSomething());
			IList<object[]> arguments = foo54.GetArgumentsForCallsMadeOn(x => x.DoSomething());
			Assert.Equal(0, arguments[0].Length);
		}

		[Fact]
		public void WhenCallingMethodWithParameters_WillReturnArgumentsInResultingArray()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.Bar("foo")).Return(1);

			Assert.Equal(1, foo54.Bar("foo"));
			Assert.Equal(0, foo54.Bar("bar"));

			IList<object[]> arguments = foo54.GetArgumentsForCallsMadeOn(x => x.Bar(Arg<string>.Matches((string s) => true)));
			Assert.Equal(2, arguments.Count);
			Assert.Equal("foo", arguments[0][0]);
			Assert.Equal("bar", arguments[1][0]);
		}

		[Fact]
		public void WhenCallingMethodWithParameters_WillReturnArgumentsInResultingArray_UsingConstraints()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.Bar("foo")).Return(1);

			Assert.Equal(1, foo54.Bar("foo"));
			Assert.Equal(0, foo54.Bar("bar"));

			IList<object[]> arguments = foo54.GetArgumentsForCallsMadeOn(x => x.Bar(null), o => o.IgnoreArguments());
			Assert.Equal(2, arguments.Count);
			Assert.Equal("foo", arguments[0][0]);
			Assert.Equal("bar", arguments[1][0]);
		}

		[Fact]
		public void CanUseNonRecordReplayModel_Expect()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Expect(x => x.DoSomething()).Return(1).Repeat.Once();
			mocks.Replay(demo);
			Assert.Equal(1, demo.DoSomething());
            demo.Expect(x => x.DoSomething()).Return(15).Repeat.Once();
			Assert.Equal(15, demo.DoSomething());

			mocks.VerifyAll();
		}

		[Fact]
		public void CanUseNonRecordReplayModel_Expect_OnVoidMethod()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Expect(x => x.DoSomethingElse());
			mocks.Replay(demo);
			demo.DoSomethingElse();
			mocks.VerifyAll();
		}


		[Fact]
		public void CanUseNonRecordReplayModel_Expect_OnVoidMethod_WhenMethodNotcall_WillFailTest()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Expect(x => x.DoSomethingElse());
			mocks.Replay(demo);

			Assert.Throws<ExpectationViolationException>(
				"IFoo54.DoSomethingElse(); Expected #1, Actual #0.",
				() => mocks.VerifyAll());
		}


		[Fact]
		public void UsingExpectWithoutSettingReturnValueThrows()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Expect(x => x.DoSomething());
			mocks.Replay(demo);
			Assert.Throws<InvalidOperationException>(
				"Method 'IFoo54.DoSomething();' requires a return value or an exception to throw.",
				() => Assert.Equal(1, demo.DoSomething()));
		}

		[Fact]
		public void CanUseNonRecordReplayModel_Stub()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Stub(x => x.DoSomething()).Return(1);

			mocks.Replay(demo);
			Assert.Equal(1, demo.DoSomething());
		}

		[Fact]
		public void CanUseStubSyntax_WithoutExplicitMockRepository()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.DoSomething()).Return(1);

			Assert.Equal(1, foo54.DoSomething());

			foo54.AssertWasCalled(x => x.DoSomething());
		}

		[Fact]
		public void CanUseStubSyntax_WithoutExplicitMockRepository_VerifyMethodWasNotCalled()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.DoSomething()).Return(1);


			foo54.AssertWasNotCalled(x => x.DoSomething());
		}

		[Fact]
		public void CanUseStubSyntax_WithoutExplicitMockRepository_VerifyMethodWasNotCalled_WillThrowIfCalled()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.DoSomething()).Return(1);

			Assert.Equal(1, foo54.DoSomething());

			Assert.Throws<ExpectationViolationException>(
				"Expected that IFoo54.DoSomething(); would not be called, but it was found on the actual calls made on the mocked object.",
				() => foo54.AssertWasNotCalled(x => x.DoSomething()));
		}

		[Fact]
		public void CanAssertOnMethodUsingDirectArgumentMatching()
		{
			var foo54 = MockRepository.GenerateMock<IFoo54>();

			foo54.Bar("blah");

			foo54.AssertWasCalled(x => x.Bar("blah"));
		}

		[Fact]
		public void CanAssertOnMethodUsingDirectArgumentMatching_WhenWrongArumentPassed()
		{
			var foo54 = MockRepository.GenerateMock<IFoo54>();

			foo54.Bar("blah");
			Assert.Throws<ExpectationViolationException>(
				"IFoo54.Bar(\"blah1\"); Expected #1, Actual #0.",
				() => foo54.AssertWasCalled(x => x.Bar("blah1")));
		}

		[Fact]
		public void CanUseExpectSyntax_WithoutExplicitMockRepository()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Expect(x => x.DoSomething()).Return(1);

			Assert.Equal(1, foo54.DoSomething());

			foo54.VerifyAllExpectations();
		}

		[Fact]
		public void CanUseExpectSyntax_WithoutExplicitMockRepository_UsingConsraints()
		{
			var foo54 = MockRepository.GenerateMock<IFoo54>();

			foo54.Expect(x => x.Bar(null)).Constraints(Text.StartsWith("boo")).Return(1);

			Assert.Equal(1, foo54.Bar("boo is a great language"));

			foo54.VerifyAllExpectations();
		}

		[Fact]
		public void CanExpectOnNumberOfCallsMade()
		{
			var foo54 = MockRepository.GenerateMock<IFoo54>();

			foo54.Expect(x => x.DoSomething()).Repeat.Twice().Return(1);

			foo54.DoSomething();
			foo54.DoSomething();

			foo54.VerifyAllExpectations();
		}

		[Fact]
		public void CanExpectOnNumberOfCallsMade_WhenRepeatCountNotMet()
		{
			var foo54 = MockRepository.GenerateMock<IFoo54>();

			foo54.Expect(x => x.DoSomething()).Repeat.Twice().Return(1);

			foo54.DoSomething();

			Assert.Throws<ExpectationViolationException>(
				"IFoo54.DoSomething(); Expected #2, Actual #1.",
				() => foo54.VerifyAllExpectations());
		}

		[Fact]
		public void CanAssertOnNumberOfCallsMade()
		{
			var foo54 = MockRepository.GenerateMock<IFoo54>();


			foo54.DoSomething();
			foo54.DoSomething();

			foo54.AssertWasCalled(x => x.DoSomething(), o => o.Repeat.Twice());
		}

		[Fact]
		public void CanAssertOnNumberOfCallsMade_WhenRepeatCountNotMet()
		{
			var foo54 = MockRepository.GenerateMock<IFoo54>();


			foo54.DoSomething();

			Assert.Throws<ExpectationViolationException>(
				"IFoo54.DoSomething(); Expected #2, Actual #1.",
				() => foo54.AssertWasCalled(x => x.DoSomething(), o => o.Repeat.Twice()));
		}

		[Fact]
		public void CanUseExpectSyntax_WithoutExplicitMockRepository_UsingConsraints_WhenViolated()
		{
			var foo54 = MockRepository.GenerateMock<IFoo54>();

			foo54.Expect(x => x.Bar(null)).Constraints(Text.StartsWith("boo")).Return(1);

			Assert.Equal(0, foo54.Bar("great test"));

			Assert.Throws<ExpectationViolationException>(
				@"IFoo54.Bar(starts with ""boo""); Expected #1, Actual #0.",
				() => foo54.VerifyAllExpectations());
		}

		[Fact]
		public void CanUseStubSyntax_WithoutExplicitMockRepository_UsingConsraints_WhenExpectationNotMatching()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.Bar(null)).Constraints(Text.StartsWith("boo")).Return(1);

			Assert.Equal(0, foo54.Bar("great test"));

			foo54.VerifyAllExpectations();
		}


		[Fact]
		public void CanUseExpectSyntax_WithoutExplicitMockRepository_WhenCallIsNotBeingMade()
		{
			var foo54 = MockRepository.GenerateMock<IFoo54>();

			foo54.Expect(x => x.DoSomething()).Return(1);

			Assert.Throws<ExpectationViolationException>("IFoo54.DoSomething(); Expected #1, Actual #0.",
			                                             () => foo54.VerifyAllExpectations());
		}


		[Fact]
		public void CanUseNonRecordReplayModel_Stub_OnVoidMethod()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Stub(x => x.DoSomethingElse()).Throw(new InvalidOperationException("foo"));
			mocks.Replay(demo);

			try
			{
				demo.DoSomethingElse();
				Assert.False(true, "Should throw");
			}
			catch (InvalidOperationException e)
			{
				Assert.Equal("foo", e.Message);
			}
		}

		[Fact]
		public void CanUseNonRecordReplayModel_Stub_AndThenVerify()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Stub(x => x.DoSomething()).Return(1);
			mocks.Replay(demo);

			Assert.Equal(1, demo.DoSomething());
			demo.AssertWasCalled(x => x.DoSomething());
		}

		[Fact]
		public void CanUseNonRecordReplayModel_Stub_AndThenVerify_WhenNotCalled_WillCauseError()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Stub(x => x.DoSomething()).Return(1);
			mocks.Replay(demo);

			Assert.Throws<ExpectationViolationException>(
				"IFoo54.DoSomething(); Expected #1, Actual #0.",
				() => demo.AssertWasCalled(x => x.DoSomething()));
		}

		[Fact]
		public void CanUseNonRecordReplayModel_Stub_WillNotThrowIfExpectationIsNotMet()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Stub(x => x.DoSomething()).Return(1);
			mocks.Replay(demo);

			mocks.VerifyAll();
		}

		[Fact]
		public void WhenNoExpectationIsSetup_WillReturnDefaultValues()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			Assert.Equal(0, demo.DoSomething());
		}

		[Fact]
		public void CanAssertOnMethodCall()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();
			mocks.Replay(demo);

			demo.DoSomething();

			demo.AssertWasCalled(x => x.DoSomething());
        }

		[Fact]
		public void CanAssertOnMethodCallUsingConstraints()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();
			mocks.Replay(demo);

			demo.Bar("blah baba");

			demo.AssertWasCalled(x => x.Bar(Arg<string>.Matches((string a) => a.StartsWith("b") && a.Contains("ba"))));
		}

        [Fact]
        public void CanAssertOnPropertyAccess()
        {
            MockRepository mocks = new MockRepository();
            IFoo54 demo = mocks.DynamicMock<IFoo54>();
            mocks.Replay(demo);

            var result = demo.FooBar;

            demo.AssertWasCalled(x => x.FooBar);
        }

        [Fact]
        public void CanAssertOnPropertyAccessWithConstraints()
        {
            MockRepository mocks = new MockRepository();
            IFoo54 demo = mocks.DynamicMock<IFoo54>();
            mocks.Replay(demo);

            var result = demo.FooBar;
            result = demo.FooBar;

            demo.AssertWasCalled(x => x.FooBar, o => o.Repeat.Twice());
        }

        [Fact]
        public void CanAssertNotCalledOnPropertyAccess()
        {
            MockRepository mocks = new MockRepository();
            IFoo54 demo = mocks.DynamicMock<IFoo54>();
            mocks.Replay(demo);

            demo.DoSomething();

            demo.AssertWasNotCalled(x => x.FooBar);
        }

#if !FOR_NET_2_0 // we can't get the structure from a delegate, as happens on 2.0, so ignore this
		[Fact]
		public void CanAssertOnMethodCallUsingConstraints_WhenMethodNotFound()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			mocks.ReplayAll();

			demo.Bar("yoho");

			Assert.Throws<ExpectationViolationException>(
				"IFoo54.Bar(a => (a.StartsWith(\"b\") && a.Contains(\"ba\"))); Expected #1, Actual #0."
				, () => demo.AssertWasCalled(x => x.Bar(Arg<string>.Matches((string a) => a.StartsWith("b") && a.Contains("ba")))));
		}
#endif

		[Fact]
		public void CannotUseRepeatAny()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();
			mocks.ReplayAll();
			Assert.Throws<InvalidOperationException>(
				"The expectation was removed from the waiting expectations list, did you call Repeat.Any() ? This is not supported in AssertWasCalled()",
				() => demo.AssertWasCalled(x => x.Bar("a"), o => o.Repeat.Any()));
		}

		[Fact]
		public void WillFailVerificationsOfMethod_IfWereNotCalled()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();
			mocks.ReplayAll();

			Assert.Throws<ExpectationViolationException>(
				"IFoo54.DoSomething(); Expected #1, Actual #0.",
				() => demo.AssertWasCalled(x => x.DoSomething()));
		}

		[Fact]
		public void WillFailVerificationsOfMethod_IfWereNotCalled_OnVoidMethod()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			mocks.ReplayAll();
			Assert.Throws<ExpectationViolationException>(
				"IFoo54.DoSomethingElse(); Expected #1, Actual #0.",
				() => demo.AssertWasCalled(x => x.DoSomethingElse()));
		}


		[Fact]
		public void CanOnlyPassSingleExpectationToVerify()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();
			mocks.Replay(demo);

			Assert.Throws<InvalidOperationException>(
				"You can only use a single expectation on AssertWasCalled(), use separate calls to AssertWasCalled() if you want to verify several expectations",
				() => demo.AssertWasCalled(x =>
				{
					x.DoSomethingElse();
					x.DoSomethingElse();
				}));
		}

		[Fact]
		public void WillRaiseErrorIfNoExpectationWasSetup()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();
			mocks.Replay(demo);

			Assert.Throws<InvalidOperationException>(
				"No expectations were setup to be verified, ensure that the method call in the action is a virtual (C#) / overridable (VB.Net) method call",
				() => demo.AssertWasCalled(x => { }));
		}

		[Fact]
		public void TypeShouldBeInferredFromMockNotReference()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<Foo54>(0);

			demo.Stub(x => x.DoSomethingElse());
			mocks.Replay(demo);

			demo.DoSomethingElse();

			demo.AssertWasCalled(x => x.DoSomethingElse());
		}

		[Fact]
		public void AssertShouldWorkWithoutStub()
		{
			var mocks = new MockRepository();
			var demo = mocks.DynamicMock<IFoo54>();
			mocks.Replay(demo);
			demo.DoSomethingElse();

			demo.AssertWasCalled(x => x.DoSomethingElse());
		}
	}

	public interface IFoo54
	{
		int DoSomething();
		void DoSomethingElse();
		int Bar(string x);
		string bar();
        string FooBar { get; set; }
	}

	public class Foo54 : IFoo54
	{
		public Foo54(int i)
		{

		}

		public virtual int DoSomething()
		{
			return 0;
		}

		public virtual void DoSomethingElse()
		{
		}

		public virtual int Bar(string x)
		{
			return 0;
		}

		#region IFoo54 Members

		public string bar()
		{
			return null;
		}

        public string FooBar { get; set; }

		#endregion
	}
}
#endif
