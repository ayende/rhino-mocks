using System;
using System.Data;
using MbUnit.Framework;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Tests.Callbacks;

namespace Rhino.Mocks.Tests.Constraints
{
	[TestFixture]
	public class ConstraintTests
	{
		private IDemo demo;
		private MockRepository mocks;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			demo = (IDemo) this.mocks.CreateMock(typeof (IDemo));
		}
#if dotNet2
		[Test]
		public void UsingPredicate()
		{
			demo.VoidStringArg(null);
			LastCall.Constraints(
				Is.Matching<string>(delegate(string s) 
				{
					return s.Length == 2;  
				}) 
				&&
				Is.Matching<string>(delegate(string s)
				{
					return s.EndsWith("b");	
				}));
			mocks.Replay(demo);
			
			demo.VoidStringArg("ab");

			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Predicate accept System.Data.DataSet but parameter is System.String which is not compatible")]
		public void UsingPredicateConstraintWhenTypesNotMatching()
		{
			demo.VoidStringArg(null);
			LastCall.Constraints(
				Is.Matching<DataSet>(delegate(DataSet s)
				{
					return false;
				}));
			mocks.Replay(demo);

			demo.VoidStringArg("ab");

			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IDemo.VoidStringArg(\"cc\"); Expected #0, Actual #1.\r\nIDemo.VoidStringArg(Predicate (ConstraintTests.JustPredicate(obj);)); Expected #1, Actual #0.")]
		public void UsingPredicateWhenExpectationViolated()
		{
			demo.VoidStringArg(null);
			LastCall.Constraints(
				Is.Matching<string>(JustPredicate)
				);
			mocks.Replay(demo);

			demo.VoidStringArg("cc");

			mocks.VerifyAll();
		}
		
		public bool JustPredicate(string s)
		{
			return false;
		}
#endif
        [Test]
        public void AndSeveralConstraings()
        {
            AbstractConstraint all = Is.NotEqual("bar") & Is.TypeOf(typeof(string)) & Is.NotNull();
            Assert.IsTrue(all.Eval("foo"));
            Assert.AreEqual("not equal to bar and type of {System.String} and not equal to null", all.Message);
        }

		 [Test]
        public void AndSeveralConstraings_WithGenerics()
        {
            AbstractConstraint all = Is.NotEqual("bar") && Is.TypeOf<string>() && Is.NotNull();
            Assert.IsTrue(all.Eval("foo"));
            Assert.AreEqual("not equal to bar and type of {System.String} and not equal to null", all.Message);
        }

		[Test]
		public void AndConstraints()
		{
			AbstractConstraint start = Text.StartsWith("Ayende"), end = Text.EndsWith("Rahien");
			AbstractConstraint combine = start & end;
			Assert.IsTrue(combine.Eval("Ayende Rahien"));
			Assert.AreEqual("starts with \"Ayende\" and ends with \"Rahien\"", combine.Message);
		}

		[Test]
		public void NotConstraint()
		{
			AbstractConstraint start = Text.StartsWith("Ayende");
			AbstractConstraint negate = !start;
			Assert.IsTrue(negate.Eval("Rahien"));
			Assert.AreEqual("not starts with \"Ayende\"", negate.Message);
		}

		[Test]
		public void OrConstraints()
		{
			AbstractConstraint start = Text.StartsWith("Ayende"), end = Text.EndsWith("Rahien");
			AbstractConstraint combine = start | end;
			Assert.IsTrue(combine.Eval("Ayende"));
			Assert.IsTrue(combine.Eval("Rahien"));
			Assert.AreEqual("starts with \"Ayende\" or ends with \"Rahien\"", combine.Message);
		}

		[Test]
		public void SettingConstraintOnAMock()
		{
			demo.VoidStringArg("Ayende");
			LastCall.On(demo).Constraints(Text.Contains("World"));
			mocks.Replay(demo);
			demo.VoidStringArg("Hello, World");
			mocks.Verify(demo);
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "IDemo.VoidStringArg(\"Hello, world\"); Expected #0, Actual #1.\r\nIDemo.VoidStringArg(contains \"World\"); Expected #1, Actual #0.")]
		public void ConstraintFailingThrows()
		{
			demo.VoidStringArg("Ayende");
			LastCall.On(demo).Constraints(Text.Contains("World"));
			mocks.Replay(demo);
			demo.VoidStringArg("Hello, world");
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "The number of constraints is not the same as the number of the method's parameters!")]
		public void ConstraintWithTooMuchForArguments()
		{
			demo.VoidStringArg("Ayende");
			LastCall.On(demo).Constraints(Text.Contains("World"), Is.Equal("Rahien"));
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "The number of constraints is not the same as the number of the method's parameters!")]
		public void ConstraintWithTooFewForArguments()
		{
			demo.VoidThreeArgs(1, "Ayende", 3.14f);
			LastCall.On(demo).Constraints(Text.Contains("World"), Is.Equal("Rahien"));
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "IDemo.VoidStringArg(contains \"World\"); Expected #1, Actual #0.")]
		public void ConstraintsThatWerentCallCauseVerifyFailure()
		{
			this.demo.VoidStringArg("Ayende");
			LastCall.On(this.demo).Constraints(Text.Contains("World"));
			this.mocks.Replay(this.demo);
			this.mocks.Verify(this.demo);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException), "This method has already been set to ConstraintsExpectation.")]
		public void AddConstraintAndThenTryToIgnoreArgs()
		{
			this.demo.VoidStringArg("Ayende");
			LastCall.On(this.demo).Constraints(Text.Contains("World")).Callback(new DelegateDefinations.StringDelegate("".StartsWith));
		}

	}
}