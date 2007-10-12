using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	using MbUnit.Framework;

	[TestFixture]
	public class FieldProblem_Andy
	{
		[Test]
		public void MockingPropertyUsingBaseKeyword()
		{
			MockRepository mocks = new MockRepository();
			SubClass mock = mocks.PartialMock<SubClass>();
			
			Expect.Call(mock.SubProperty)
				.Return("Foo")
				.Repeat.Any();
			Expect.Call(mock.BaseProperty)
				.Return("Foo2")
				.Repeat.Any();

			mocks.ReplayAll();
			
			Assert.AreEqual("Foo", mock.SubProperty);
			Assert.AreEqual("Foo2", mock.BaseProperty);
			
			mocks.VerifyAll();
		}
	}

	public abstract class BaseClass
	{
		private string property = null;
		public virtual string BaseProperty
		{
			get { return property; }
			set { this.property = value; }
		}
	}

	public class SubClass : BaseClass
	{
		public virtual string SubProperty
		{
			get { return base.BaseProperty; }
		}

		public override string BaseProperty
		{
			get
			{
				return base.BaseProperty;
			}
			set
			{
				base.BaseProperty = value;
			}
		}

	}
}
