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
using MbUnit.Framework;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
	[TestFixture]
	public class RecordingMocks
	{
		[Test]
		public void CanGetMockRepositoryForMock()
		{
			MockRepository repository = new MockRepository();
			var foo = repository.StrictMock<IFoo54>();
			Assert.AreSame(repository, foo.GetMockRepository());
		}

		[Test]
		public void CanResetStubAndReuseIt()
		{
			var foo = MockRepository.GenerateStub<IFoo54>();

			foo.Stub(x => x.bar()).Return("open").Repeat.Any();
			// several calls to 'foo.bar()
			Assert.AreEqual(foo.bar(), "open");
			Assert.AreEqual(foo.bar(), "open");
			
			foo.BackToRecord();

			foo.Stub(x => x.bar()).Return("closed").Repeat.Any();
			// several calls to 'foo.bar()
			Assert.AreEqual(foo.bar(), "closed");
			Assert.AreEqual(foo.bar(), "closed");
		}

		[Test]
		public void CanAssertOnCountOfCallsMade()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.DoSomething()).Return(1);

			Assert.AreEqual(1, foo54.DoSomething());

			foo54.AssertWasCalled(x => x.DoSomething());
			Assert.AreEqual(1, foo54.GetArgumentsForCallsMadeOn(x => x.DoSomething()).Count);
		}

		[Test]
		public void WhenCallingMethodWithNoParameters_WillReturnZeroLengthArrayForEachCall()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.DoSomething()).Return(1);

			Assert.AreEqual(1, foo54.DoSomething());

			foo54.AssertWasCalled(x => x.DoSomething());
			IList<object[]> arguments = foo54.GetArgumentsForCallsMadeOn(x => x.DoSomething());
			Assert.AreEqual(0, arguments[0].Length);
		}

		[Test]
		public void WhenCallingMethodWithParameters_WillReturnArgumentsInResultingArray()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.Bar("foo")).Return(1);

			Assert.AreEqual(1, foo54.Bar("foo"));
			Assert.AreEqual(0, foo54.Bar("bar"));

			IList<object[]> arguments = foo54.GetArgumentsForCallsMadeOn(x => x.Bar(Arg<string>.Matches(s => true)));
			Assert.AreEqual(2, arguments.Count);
			Assert.AreEqual("foo", arguments[0][0]);
			Assert.AreEqual("bar", arguments[1][0]);
		}

		[Test]
		public void WhenCallingMethodWithParameters_WillReturnArgumentsInResultingArray_UsingConstraints()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.Bar("foo")).Return(1);

			Assert.AreEqual(1, foo54.Bar("foo"));
			Assert.AreEqual(0, foo54.Bar("bar"));

			IList<object[]> arguments = foo54.GetArgumentsForCallsMadeOn(x => x.Bar(null), o => o.IgnoreArguments());
			Assert.AreEqual(2, arguments.Count);
			Assert.AreEqual("foo", arguments[0][0]);
			Assert.AreEqual("bar", arguments[1][0]);
		}

		[Test]
		public void CanUseNonRecordReplayModel_Expect()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Expect(x => x.DoSomething()).Return(1);
			Assert.AreEqual(1, demo.DoSomething());
			demo.Expect(x => x.DoSomething()).Return(15);
			Assert.AreEqual(15, demo.DoSomething());

			mocks.VerifyAll();
		}

		[Test]
		public void CanUseNonRecordReplayModel_Expect_OnVoidMethod()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Expect(x => x.DoSomethingElse());
			demo.DoSomethingElse();
			mocks.VerifyAll();
		}


		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IFoo54.DoSomethingElse(); Expected #1, Actual #0.")]
		public void CanUseNonRecordReplayModel_Expect_OnVoidMethod_WhenMethodNotcall_WillFailTest()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Expect(x => x.DoSomethingElse());

			mocks.VerifyAll();
		}


		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Method 'IFoo54.DoSomething();' requires a return value or an exception to throw.")]
		public void UsingExpectWithoutSettingReturnValueThrows()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Expect(x => x.DoSomething());
			Assert.AreEqual(1, demo.DoSomething());
		}

		[Test]
		public void CanUseNonRecordReplayModel_Stub()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();
			mocks.Replay(demo);

			demo.Stub(x => x.DoSomething()).Return(1);

			Assert.AreEqual(1, demo.DoSomething());
		}

		[Test]
		public void CanUseStubSyntax_WithoutExplicitMockRepository()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.DoSomething()).Return(1);

			Assert.AreEqual(1, foo54.DoSomething());

			foo54.AssertWasCalled(x => x.DoSomething());
		}

		[Test]
		public void CanUseStubSyntax_WithoutExplicitMockRepository_VerifyMethodWasNotCalled()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.DoSomething()).Return(1);


			foo54.AssertWasNotCalled(x => x.DoSomething());
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "Expected that IFoo54.DoSomething(); would not be called, but it was found on the actual calls made on the mocked object.")]
		public void CanUseStubSyntax_WithoutExplicitMockRepository_VerifyMethodWasNotCalled_WillThrowIfCalled()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.DoSomething()).Return(1);

			Assert.AreEqual(1, foo54.DoSomething());

			foo54.AssertWasNotCalled(x => x.DoSomething());
		}

		[Test]
		public void CanAssertOnMethodUsingDirectArgumentMatching()
		{
			var foo54 = MockRepository.GenerateMock<IFoo54>();

			foo54.Bar("blah");

			foo54.AssertWasCalled(x => x.Bar("blah"));
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException),
			"Expected that IFoo54.Bar(\"blah1\"); would be called, but it was not found on the actual calls made on the mocked object.")]
		public void CanAssertOnMethodUsingDirectArgumentMatching_WhenWrongArumentPassed()
		{
			var foo54 = MockRepository.GenerateMock<IFoo54>();

			foo54.Bar("blah");

			foo54.AssertWasCalled(x => x.Bar("blah1"));
		}

		[Test]
		public void CanUseExpectSyntax_WithoutExplicitMockRepository()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Expect(x => x.DoSomething()).Return(1);

			Assert.AreEqual(1, foo54.DoSomething());

			foo54.VerifyAllExpectations();
		}

		[Test]
		public void CanUseExpectSyntax_WithoutExplicitMockRepository_UsingConsraints()
		{
			var foo54 = MockRepository.GenerateMock<IFoo54>();

			foo54.Expect(x => x.Bar(null)).Constraints(Text.StartsWith("boo")).Return(1);

			Assert.AreEqual(1, foo54.Bar("boo is a great language"));

			foo54.VerifyAllExpectations();
		}


		[Test]
		[ExpectedException(typeof(ExpectationViolationException), @"IFoo54.Bar(starts with ""boo""); Expected #1, Actual #0.")]
		public void CanUseExpectSyntax_WithoutExplicitMockRepository_UsingConsraints_WhenViolated()
		{
			var foo54 = MockRepository.GenerateMock<IFoo54>();

			foo54.Expect(x => x.Bar(null)).Constraints(Text.StartsWith("boo")).Return(1);

			Assert.AreEqual(0, foo54.Bar("great test"));

			foo54.VerifyAllExpectations();
		}

		[Test]
		public void CanUseStubSyntax_WithoutExplicitMockRepository_UsingConsraints_WhenExpectationNotMatching()
		{
			var foo54 = MockRepository.GenerateStub<IFoo54>();

			foo54.Stub(x => x.Bar(null)).Constraints(Text.StartsWith("boo")).Return(1);

			Assert.AreEqual(0, foo54.Bar("great test"));

			foo54.VerifyAllExpectations();
		}


		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IFoo54.DoSomething(); Expected #1, Actual #0.")]
		public void CanUseExpectSyntax_WithoutExplicitMockRepository_WhenCallIsNotBeingMade()
		{
			var foo54 = MockRepository.GenerateMock<IFoo54>();

			foo54.Expect(x => x.DoSomething()).Return(1);

			foo54.VerifyAllExpectations();
		}


		[Test]
		public void CanUseNonRecordReplayModel_Stub_OnVoidMethod()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Stub(x => x.DoSomethingElse()).Throw(new InvalidOperationException("foo"));

			try
			{
				demo.DoSomethingElse();
				Assert.Fail("Should throw");
			}
			catch (InvalidOperationException e)
			{
				Assert.AreEqual("foo", e.Message);
			}
		}

		[Test]
		public void CanUseNonRecordReplayModel_Stub_AndThenVerify()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Stub(x => x.DoSomething()).Return(1);

			Assert.AreEqual(1, demo.DoSomething());
			demo.AssertWasCalled(x => x.DoSomething());
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "Expected that IFoo54.DoSomething(); would be called, but it was not found on the actual calls made on the mocked object.")]
		public void CanUseNonRecordReplayModel_Stub_AndThenVerify_WhenNotCalled_WillCauseError()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Stub(x => x.DoSomething()).Return(1);

			demo.AssertWasCalled(x => x.DoSomething());
		}

		[Test]
		public void CanUseNonRecordReplayModel_Stub_WillNotThrowIfExpectationIsNotMet()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			demo.Stub(x => x.DoSomething()).Return(1);

			mocks.VerifyAll();
		}

		[Test]
		public void WhenNoExpectationIsSetup_WillReturnDefaultValues()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			Assert.AreEqual(0, demo.DoSomething());
		}

		[Test]
		public void CanAssertOnMethodCall()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();
			mocks.Replay(demo);

			demo.DoSomething();

			demo.AssertWasCalled(x => x.DoSomething());
		}

		[Test]
		public void CanAssertOnMethodCallUsingConstraints()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();
			mocks.Replay(demo);

			demo.Bar("blah baba");

			demo.AssertWasCalled(x => x.Bar(Arg<string>.Matches(a => a.StartsWith("b") && a.Contains("ba"))));
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "Expected that IFoo54.Bar(a => (a.StartsWith(\"b\") && a.Contains(\"ba\"))); would be called, but it was not found on the actual calls made on the mocked object.")]
		public void CanAssertOnMethodCallUsingConstraints_WhenMethodNotFound()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			mocks.ReplayAll();

			demo.Bar("yoho");

			demo.AssertWasCalled(x => x.Bar(Arg<string>.Matches(a => a.StartsWith("b") && a.Contains("ba"))));
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "The expectation was removed from the waiting expectations list, did you call Repeat.Any() ? This is not supported in AssertWasCalled()")]
		public void CannotUseRepeatAny()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();
			mocks.ReplayAll();
			demo.AssertWasCalled(x => x.Bar("a"), o => o.Repeat.Any());
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Cannot pass explicit delegate to setup the expectation and also use Arg<T>.Matches")]
		public void CannotSpecifyConstraintsAndArgMatching()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();
			mocks.ReplayAll();
			demo.AssertWasCalled(x => x.Bar(Arg<string>.Matches(a => a.StartsWith("b") && a.Contains("ba"))), o => o.Repeat.Once());
		}


		// Add repeat never

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "Expected that IFoo54.DoSomething(); would be called, but it was not found on the actual calls made on the mocked object.")]
		public void WillFailVerificationsOfMethod_IfWereNotCalled()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();
			mocks.ReplayAll();

			demo.AssertWasCalled(x => x.DoSomething());
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "Expected that IFoo54.DoSomethingElse(); would be called, but it was not found on the actual calls made on the mocked object.")]
		public void WillFailVerificationsOfMethod_IfWereNotCalled_OnVoidMethod()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

			mocks.ReplayAll();
			demo.AssertWasCalled(x => x.DoSomethingElse());
		}


		[Test]
		[ExpectedException(typeof(InvalidOperationException), "You can only use a single expectation on AssertWasCalled(), use separate calls to AssertWasCalled() if you want to verify several expectations")]
		public void CanOnlyPassSingleExpectationToVerify()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();
			mocks.Replay(demo);

			demo.AssertWasCalled(x =>
							{
								x.DoSomethingElse();
								x.DoSomethingElse();
							});
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "No expectations were setup to be verified, ensure that the method call in the action is a virtual (C#) / overridable (VB.Net) method call")]
		public void WillRaiseErrorIfNoExpectationWasSetup()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();
			mocks.Replay(demo);

			demo.AssertWasCalled(x => { });
		}

		[Test]
		public void TypeShouldBeInferredFromMockNotReference()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<Foo54>(0);

			demo.Stub(x => x.DoSomethingElse());

			demo.DoSomethingElse();

			demo.AssertWasCalled(x => x.DoSomethingElse());
		}

		[Test]
		[Ignore]
		public void AssertShouldWorkWithoutStub()
		{
			MockRepository mocks = new MockRepository();
			IFoo54 demo = mocks.DynamicMock<IFoo54>();

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

		#endregion
	}
}
#endif
