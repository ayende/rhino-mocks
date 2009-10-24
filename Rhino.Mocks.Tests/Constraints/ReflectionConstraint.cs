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


using System;
using System.Collections;
using System.Reflection;
using Xunit;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.Constraints
{
	
	public class ReflectionConstraint
	{
		[Fact]
		public void IsTypeOf()
		{
			AbstractConstraint typeOf = Is.TypeOf(typeof (int));
			Assert.True(typeOf.Eval(3));
			Assert.False(typeOf.Eval(""));
			Assert.False(typeOf.Eval(null));
			Assert.Equal("type of {System.Int32}", typeOf.Message);
		}

		[Fact]
		public void PropertyValue()
		{
			AbstractConstraint constraint = Property.Value("Length", 6);
			Assert.True(constraint.Eval("Ayende"));
			Assert.False(constraint.Eval(new ArrayList()));
			Assert.Equal("property 'Length' equal to 6", constraint.Message);
		}


		[Fact]
		public void PropertyNull()
		{
			Exception withoutInner = new Exception(), withInner = new Exception("", withoutInner);
			AbstractConstraint constraint = Property.IsNull("InnerException");
			Assert.True(constraint.Eval(withoutInner));
			Assert.False(constraint.Eval(withInner));
			Assert.Equal("property 'InnerException' equal to null", constraint.Message);
		}

        [Fact]
        public void PropertyConstraint()
        {
            Exception innerException = new Exception("This is the inner exception");
            Exception outerException = new Exception("This is the outer exception", innerException);

            Assert.True(
                Property.ValueConstraint("InnerException",
                    Property.ValueConstraint("Message",
                        Text.Contains("inner")
                    )
                ).Eval(outerException)
            );
            Assert.False(
                Property.ValueConstraint("InnerException",
                    Property.ValueConstraint("Message",
                        Text.Contains("outer")
                    )
                ).Eval(outerException)
            );
        }

		[Fact]
		public void PropertyNotNull()
		{
			Exception withoutInner = new Exception(), withInner = new Exception("", withoutInner);
			AbstractConstraint constraint = Property.IsNotNull("InnerException");
			Assert.False(constraint.Eval(withoutInner));
			Assert.True(constraint.Eval(withInner));
			Assert.Equal("not property 'InnerException' equal to null", constraint.Message);
		}

        [Fact]
        public void AmbiguousPropertyAccess()
        {
            DerivedPropertyAccessFodder o = new DerivedPropertyAccessFodder();

            AbstractConstraint constraint = Property.Value("Property", "4");

            // This will fail with an AmbiguousMatchException because 'Property' is not
            // unique: there are two public Property properties.
        	Assert.Throws<AmbiguousMatchException>(() => Assert.True(constraint.Eval(o)));
        }

        [Fact]
        public void DisambiguatedPropertyEqualAccess()
        {
            DerivedPropertyAccessFodder o = new DerivedPropertyAccessFodder();

            AbstractConstraint constraint;
            
            constraint = Property.Value(typeof(BasePropertyAccessFodder), "Property", 1);
            Assert.True(constraint.Eval(o), "Base True");
            constraint = Property.Value(typeof(BasePropertyAccessFodder), "Property", 0);
            Assert.False(constraint.Eval(o), "Base False");

            constraint = Property.Value(typeof(IPropertyAccessFodder1), "Property", 2);
            Assert.True(constraint.Eval(o), "Interface1 True");
            constraint = Property.Value(typeof(IPropertyAccessFodder1), "Property", 0);
            Assert.False(constraint.Eval(o), "Interface1 False");

            constraint = Property.Value(typeof(IPropertyAccessFodder2), "Property", null);
            Assert.True(constraint.Eval(o), "Interface2 True");
            constraint = Property.Value(typeof(IPropertyAccessFodder2), "Property", 0);
            Assert.False(constraint.Eval(o), "Interface2 False");

            constraint = Property.Value(typeof(DerivedPropertyAccessFodder), "Property", "4");
            Assert.True(constraint.Eval(o), "Derived True");
            constraint = Property.Value(typeof(DerivedPropertyAccessFodder), "Property", "0");
            Assert.False(constraint.Eval(o), "Derived False");

            // Also test that we can use the disambiguation to access private properties
            constraint = Property.Value(typeof(BasePropertyAccessFodder), "PrivateProperty", 5);
            Assert.True(constraint.Eval(o), "BasePrivate True");
            constraint = Property.Value(typeof(BasePropertyAccessFodder), "PrivateProperty", 0);
            Assert.False(constraint.Eval(o), "BasePrivate False");

            constraint = Property.Value(typeof(DerivedPropertyAccessFodder), "PrivateProperty", 6);
            Assert.True(constraint.Eval(o), "DerivedPrivate True");
            constraint = Property.Value(typeof(DerivedPropertyAccessFodder), "PrivateProperty", 0);
            Assert.False(constraint.Eval(o), "DerivedPrivate False");
        }

        [Fact]
        public void DisambiguatedPropertyConstraintAccess()
        {
            DerivedPropertyAccessFodder o = new DerivedPropertyAccessFodder();

            AbstractConstraint constraint;

            constraint = Property.ValueConstraint(typeof(BasePropertyAccessFodder), "Property", Is.Equal(1));
            Assert.True(constraint.Eval(o), "Base True");
            constraint = Property.ValueConstraint(typeof(BasePropertyAccessFodder), "Property", Is.Equal(0));
            Assert.False(constraint.Eval(o), "Base False");

            constraint = Property.ValueConstraint(typeof(IPropertyAccessFodder1), "Property", Is.Equal(2));
            Assert.True(constraint.Eval(o), "Interface1 True");
            constraint = Property.ValueConstraint(typeof(IPropertyAccessFodder1), "Property", Is.Equal(0));
            Assert.False(constraint.Eval(o), "Interface1 False");

            constraint = Property.ValueConstraint(typeof(IPropertyAccessFodder2), "Property", Is.Equal(null));
            Assert.True(constraint.Eval(o), "Interface2 True");
            constraint = Property.ValueConstraint(typeof(IPropertyAccessFodder2), "Property", Is.Equal(0));
            Assert.False(constraint.Eval(o), "Interface2 False");

            constraint = Property.ValueConstraint(typeof(DerivedPropertyAccessFodder), "Property", Is.Equal("4"));
            Assert.True(constraint.Eval(o), "Derived True");
            constraint = Property.ValueConstraint(typeof(DerivedPropertyAccessFodder), "Property", Is.Equal("0"));
            Assert.False(constraint.Eval(o), "Derived False");

            // Also test that we can use the disambiguation to access private properties
            constraint = Property.ValueConstraint(typeof(BasePropertyAccessFodder), "PrivateProperty", Is.Equal(5));
            Assert.True(constraint.Eval(o), "BasePrivate True");
            constraint = Property.ValueConstraint(typeof(BasePropertyAccessFodder), "PrivateProperty", Is.Equal(0));
            Assert.False(constraint.Eval(o), "BasePrivate False");

            constraint = Property.ValueConstraint(typeof(DerivedPropertyAccessFodder), "PrivateProperty", Is.Equal(6));
            Assert.True(constraint.Eval(o), "DerivedPrivate True");
            constraint = Property.ValueConstraint(typeof(DerivedPropertyAccessFodder), "PrivateProperty", Is.Equal(0));
            Assert.False(constraint.Eval(o), "DerivedPrivate False");
        }

		[Fact]
		public void DisambiguatedPropertyIsNullAccess()
        {
            DerivedPropertyAccessFodder o = new DerivedPropertyAccessFodder();

            AbstractConstraint constraint;

            constraint = Property.IsNull(typeof(BasePropertyAccessFodder), "Property");
            Assert.False(constraint.Eval(o), "Base False");

            constraint = Property.IsNull(typeof(IPropertyAccessFodder2), "Property");
            Assert.True(constraint.Eval(o), "Interface2 True");
        }

		[Fact]
		public void DisambiguatedPropertyIsNotNullAccess()
        {
            DerivedPropertyAccessFodder o = new DerivedPropertyAccessFodder();

            AbstractConstraint constraint;

            constraint = Property.IsNotNull(typeof(BasePropertyAccessFodder), "Property");
            Assert.True(constraint.Eval(o), "Base True");

            constraint = Property.IsNotNull(typeof(IPropertyAccessFodder2), "Property");
            Assert.False(constraint.Eval(o), "Interface2 False");
        }

        #region PropertyAccess fodder types
        public class BasePropertyAccessFodder
        {
            public int Property { get { return 1; } }
            private int PrivateProperty { get { return 5; } }
        }

        public interface IPropertyAccessFodder1
        {
            int Property { get; }
        }

        public interface IPropertyAccessFodder2
        {
            object Property { get; }
        }

        public class DerivedPropertyAccessFodder : BasePropertyAccessFodder, IPropertyAccessFodder1, IPropertyAccessFodder2
        {
            int IPropertyAccessFodder1.Property { get { return 2; } }
            object IPropertyAccessFodder2.Property { get { return null; } }
            public new string Property { get { return "4"; } }
            private int PrivateProperty { get { return 6; } }
        }
        #endregion

        #region PublicFieldConstraints

		[Fact]
		public void PublicFieldValue()
        {
            string barTestValue = "my bar";

            AbstractConstraint constraint = PublicField.Value("BarField", barTestValue);
            Assert.True(constraint.Eval(new FooMessage(string.Empty, barTestValue)), "Returned false when field was correct");
            Assert.False(constraint.Eval(new FooMessage(string.Empty, "your bar")), "Returned true when field was incorrect");
        }

		[Fact]
		public void PublicFieldValueDoesNotVerifyProperties()
        {
            string fooTestValue = "my foo";

            AbstractConstraint constraint = PublicField.Value("FooProperty", fooTestValue);
            Assert.False(constraint.Eval(new FooMessage(fooTestValue, string.Empty)), "Returned false when trying to validate a property rather than a field.");
        }

		[Fact]
		public void PublicFieldValueOnImplementedInterfaceType()
        {
            string barTestValue = "my bar";
            BarMessage message = new BarMessage(string.Empty, barTestValue);

            AbstractConstraint baseConstraint = PublicField.Value(typeof(FooMessage), "BarField", barTestValue);
            Assert.True(baseConstraint.Eval(message), "Returned false when field was correct and field was declared in supplied type..");

            AbstractConstraint constraint = PublicField.Value(typeof(BarMessage), "BarField", barTestValue);
            Assert.False(constraint.Eval(message), "Returned true when field was correct but type was not declared on the suppleid type.");
        }

		[Fact]
		public void PublicFieldValueConstraint()
        {
            FooMessage message = new FooMessage("my foo", "my bar");
            
            Assert.True(PublicField.ValueConstraint("BarField", Text.Contains("bar")).Eval(message), "Returned false when supplied constraint was valid.");
            Assert.False(PublicField.ValueConstraint("BarField", Text.Contains("foo")).Eval(message), "Returned true when supplied constraint was invalid.");
        }

		[Fact]
		public void PublicFieldNull()
        {
            string barTestValue = "my bar";

            AbstractConstraint constraint = PublicField.IsNull("BarField");
            Assert.True(constraint.Eval(new FooMessage(null, null)), "Returned false when field was null.");
            Assert.False(constraint.Eval(new FooMessage(string.Empty, barTestValue)), "Returned true when field was not null");
        }

		[Fact]
		public void PublicFieldNotNull()
        {
            string barTestValue = "my bar";

            AbstractConstraint constraint = PublicField.IsNotNull("BarField");
            Assert.True(constraint.Eval(new FooMessage(string.Empty, barTestValue)), "Returned false when field was not null.");
            Assert.False(constraint.Eval(new FooMessage(null, null)), "Returned true when field was null");
        }

        #region Types for testing PublicFieldConstraints

        public interface IMessage
        {
            string CommonName { get; }
        }

        public class FooMessage : IMessage
        {
            private string _commonName;
            private string _fooProperty;

            public string FooProperty
            {
                get { return this._fooProperty; }
                set { this._fooProperty = value; }
            }

            public string BarField;

            public FooMessage(string commonName)
            {
                this._commonName = commonName;
            }

            public FooMessage(string fooProperty, string barField)
            {
                this.FooProperty = fooProperty;
                this.BarField = barField;
            }

            #region IMessage Members

            string IMessage.CommonName
            {
                get { return this._commonName; }
            }

            #endregion
        }

        public class BarMessage : FooMessage
        {
            public BarMessage(string commonName) : base(commonName) { }
            public BarMessage(string fooProperty, string barField) : base(fooProperty, barField) { }
        }

        #endregion

        #endregion
    }
}
