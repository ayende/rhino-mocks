using System;
using MbUnit.Framework;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.Constraints
{

	[TestFixture]
	public class ContraintsErrorConditions
	{
		[Test]
		public void ComparingConstraintsWhenParameterIsNotIComparable()
		{
			Assert.IsFalse(Is.GreaterThan(4).Eval(new object()));
		}

		[Test]
		public void TextWhenParameterIsNotString()
		{
			Assert.IsFalse(Text.Contains("one").Eval(1));
			Assert.IsFalse(Text.EndsWith("one").Eval(1));
			Assert.IsFalse(Text.StartsWith("one").Eval(1));
			Assert.IsFalse(Text.Like("one").Eval(1));
		}


		[Test]
		public void PropertyIsWhenParameterIsNull()
		{
			Assert.IsFalse(Property.Value("Capacity",500).Eval(null));
		}
	}
}
