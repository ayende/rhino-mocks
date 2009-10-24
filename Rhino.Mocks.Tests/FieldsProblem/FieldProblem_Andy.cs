using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	using Xunit;

	
	public class FieldProblem_Andy
	{
		[Fact]
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
			
			Assert.Equal("Foo", mock.SubProperty);
			Assert.Equal("Foo2", mock.BaseProperty);
			
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
