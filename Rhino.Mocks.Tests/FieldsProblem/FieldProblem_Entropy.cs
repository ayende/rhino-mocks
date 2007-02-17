using System;
using System.Text;
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	[TestFixture]
	public class FieldProblem_Entropy
	{
		public interface IMyObject
		{
			void DoSomething();
			void DoSomethingElse();
			object SomeProperty { get; set; }
		}

		[Test]
		public void NestedOrderedAndAtLeastOnce()
		{
			MockRepository mocks = new MockRepository();
			IMyObject myObject = mocks.CreateMock(typeof(IMyObject)) as IMyObject;

			using (mocks.Ordered())
			{
				using (mocks.Ordered()) // <-- Works if removed. Unordered does not work too.
				{
					Expect.Call(myObject.SomeProperty).Return(null).Repeat.AtLeastOnce();
				}
				myObject.DoSomething();
			}

			mocks.ReplayAll();

			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			myObject.DoSomething();

			mocks.VerifyAll();
		}


		[Test]
		[ExpectedException(typeof(ExpectationViolationException), "IMyObject.DoSomething(); Expected #1, Actual #0.")]
		public void ShouldFailInNestedOrderringIfMethodWasNotCalled()
		{
			MockRepository mocks = new MockRepository();
			IMyObject myObject = mocks.CreateMock(typeof(IMyObject)) as IMyObject;

			using (mocks.Ordered())
			{
				using (mocks.Ordered()) 
				{
					myObject.DoSomethingElse();
					LastCall.Repeat.AtLeastOnce();
				}
				myObject.DoSomething(); 
			}

			mocks.ReplayAll();
			myObject.DoSomethingElse(); 
			mocks.VerifyAll();
		}

		[Test]
		public void NestedInorderedAndAtLeastOnce()
		{
			MockRepository mocks = new MockRepository();
			IMyObject myObject = mocks.CreateMock(typeof(IMyObject)) as IMyObject;

			using (mocks.Ordered())
			{
				using (mocks.Unordered()) // <-- Works only if ordered
				{
					Expect.Call(myObject.SomeProperty).Return(null).Repeat.AtLeastOnce();
				}
				myObject.DoSomething();
			}

			mocks.ReplayAll();

			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			myObject.DoSomething();
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), @"IMyObject.DoSomething(); Expected #0, Actual #1.")]
		public void UnorderedAndAtLeastOnce_CallingAnExtraMethod()
		{
			MockRepository mocks = new MockRepository();
			IMyObject myObject = mocks.CreateMock(typeof(IMyObject)) as IMyObject;

			using (mocks.Unordered())
			{
				Expect.Call(myObject.SomeProperty).Return(null).Repeat.AtLeastOnce();
			}
			myObject.DoSomethingElse();

			mocks.ReplayAll();

			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			myObject.DoSomething();

			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException), @"IMyObject.DoSomething(); Expected #0, Actual #1.")]
		public void OrderedAndAtLeastOnce_CallingAnExtraMethod()
		{
			MockRepository mocks = new MockRepository();
			IMyObject myObject = mocks.CreateMock(typeof(IMyObject)) as IMyObject;

			using (mocks.Ordered())
			{
				Expect.Call(myObject.SomeProperty).Return(null).Repeat.AtLeastOnce();
			}
			myObject.DoSomethingElse();

			mocks.ReplayAll();

			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			Assert.IsNull(myObject.SomeProperty);
			myObject.DoSomething();

			mocks.VerifyAll();
		}

	}
}
