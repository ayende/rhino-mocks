#if DOTNET35
using System;
using MbUnit.Framework;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
    [TestFixture]
    public class RecordingMocks
    {
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
        public void CanAssertOnMethodUsingDirectArgumentMatching()
        {
            var foo54 = MockRepository.GenerateMock<IFoo54>();

            foo54.Bar("blah");

            foo54.AssertWasCalled(x => x.Bar("blah"));
        }

        [Test]
        [ExpectedException(typeof(ExpectationViolationException),
            "Expectd that IFoo54.Bar(\"blah1\"); would be called, but is was it was not found on the actual calls made on the mocked object")]
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
        [ExpectedException(typeof(ExpectationViolationException),@"IFoo54.Bar(starts with ""boo""); Expected #1, Actual #0.")]
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
            IFoo54 demo = mocks.DynamicMock<IFoo54 >();

            demo.Stub(x => x.DoSomething()).Return(1);

            Assert.AreEqual(1, demo.DoSomething());
            demo.AssertWasCalled(x => x.DoSomething());
        }

        [Test]
        [ExpectedException(typeof(ExpectationViolationException), "Expectd that IFoo54.DoSomething(); would be called, but is was it was not found on the actual calls made on the mocked object")]
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
        [ExpectedException(typeof(ExpectationViolationException), "Expectd that IFoo54.Bar(a => (a.StartsWith(\"b\") && a.Contains(\"ba\"))); would be called, but is was it was not found on the actual calls made on the mocked object")]
        public void CanAssertOnMethodCallUsingConstraints_WhenMethodNotFound()
        {
            MockRepository mocks = new MockRepository();
            IFoo54 demo = mocks.DynamicMock<IFoo54>();

            demo.Bar("yoho");

            demo.AssertWasCalled(x => x.Bar(Arg<string>.Matches(a => a.StartsWith("b") && a.Contains("ba"))));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "The expectation was removed from the waiting expectations list, did you call Repeat.Any() ? This is not supported in AssertWasCalled()")]
        public void CannotUseRepeatAny()
        {
            MockRepository mocks = new MockRepository();
            IFoo54 demo = mocks.DynamicMock<IFoo54>();

            demo.AssertWasCalled(x => x.Bar("a"), o => o.Repeat.Any());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), "Cannot pass explicit delegate to setup the expectation and also use Arg<T>.Matches")]
        public void CannotSpecifyConstraintsAndArgMatching()
        {
            MockRepository mocks = new MockRepository();
            IFoo54 demo = mocks.DynamicMock<IFoo54>();

            demo.AssertWasCalled(x => x.Bar(Arg<string>.Matches(a => a.StartsWith("b") && a.Contains("ba"))), o => o.Repeat.Once() );
        }
        

        // Add repeat never

        [Test]
        [ExpectedException(typeof(ExpectationViolationException), "Expectd that IFoo54.DoSomething(); would be called, but is was it was not found on the actual calls made on the mocked object")]
        public void WillFailVerificationsOfMethod_IfWereNotCalled()
        {
            MockRepository mocks = new MockRepository();
            IFoo54 demo = mocks.DynamicMock<IFoo54>();

            demo.AssertWasCalled(x => x.DoSomething());
        }

        [Test]
        [ExpectedException(typeof(ExpectationViolationException), "Expectd that IFoo54.DoSomethingElse(); would be called, but is was it was not found on the actual calls made on the mocked object")]
        public void WillFailVerificationsOfMethod_IfWereNotCalled_OnVoidMethod()
        {
            MockRepository mocks = new MockRepository();
            IFoo54 demo = mocks.DynamicMock<IFoo54>();

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
    }
}
#endif
