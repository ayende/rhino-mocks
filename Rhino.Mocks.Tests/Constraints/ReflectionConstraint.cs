using System;
using System.Collections;
using MbUnit.Framework;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Tests.Constraints
{
	[TestFixture]
	public class ReflectionConstraint
	{
		[Test]
		public void IsTypeOf()
		{
			AbstractConstraint typeOf = Is.TypeOf(typeof (int));
			Assert.IsTrue(typeOf.Eval(3));
			Assert.IsFalse(typeOf.Eval(""));
			Assert.IsFalse(typeOf.Eval(null));
			Assert.AreEqual("type of {System.Int32}", typeOf.Message);
		}

		[Test]
		public void PropertyValue()
		{
			AbstractConstraint constraint = Property.Value("Length", 6);
			Assert.IsTrue(constraint.Eval("Ayende"));
			Assert.IsFalse(constraint.Eval(new ArrayList()));
			Assert.AreEqual("property 'Length' equal to 6", constraint.Message);
		}


		[Test]
		public void PropertyNull()
		{
			Exception withoutInner = new Exception(), withInner = new Exception("", withoutInner);
			AbstractConstraint constraint = Property.IsNull("InnerException");
			Assert.IsTrue(constraint.Eval(withoutInner));
			Assert.IsFalse(constraint.Eval(withInner));
			Assert.AreEqual("property 'InnerException' equal to null", constraint.Message);
		}

        [Test]
        public void PropertyConstraint()
        {
            Exception innerException = new Exception("This is the inner exception");
            Exception outerException = new Exception("This is the outer exception", innerException);

            Assert.IsTrue(
                Property.ValueConstraint("InnerException",
                    Property.ValueConstraint("Message",
                        Text.Contains("inner")
                    )
                ).Eval(outerException)
            );
            Assert.IsFalse(
                Property.ValueConstraint("InnerException",
                    Property.ValueConstraint("Message",
                        Text.Contains("outer")
                    )
                ).Eval(outerException)
            );
        }

		[Test]
		public void PropertyNotNull()
		{
			Exception withoutInner = new Exception(), withInner = new Exception("", withoutInner);
			AbstractConstraint constraint = Property.IsNotNull("InnerException");
			Assert.IsFalse(constraint.Eval(withoutInner));
			Assert.IsTrue(constraint.Eval(withInner));
			Assert.AreEqual("not property 'InnerException' equal to null", constraint.Message);
		}

        [Test]
        [ExpectedException(typeof(System.Reflection.AmbiguousMatchException))]
        public void AmbiguousPropertyAccess()
        {
            DerivedPropertyAccessFodder o = new DerivedPropertyAccessFodder();

            AbstractConstraint constraint = Property.Value("Property", "4");

            // This will fail with an AmbiguousMatchException because 'Property' is not
            // unique: there are two public Property properties.
            Assert.IsTrue(constraint.Eval(o));
        }

        [Test(Description="Tests that Property.Value() can disambiguate between properties of the same name")]
        public void DisambiguatedPropertyEqualAccess()
        {
            DerivedPropertyAccessFodder o = new DerivedPropertyAccessFodder();

            AbstractConstraint constraint;
            
            constraint = Property.Value(typeof(BasePropertyAccessFodder), "Property", 1);
            Assert.IsTrue(constraint.Eval(o), "Base True");
            constraint = Property.Value(typeof(BasePropertyAccessFodder), "Property", 0);
            Assert.IsFalse(constraint.Eval(o), "Base False");

            constraint = Property.Value(typeof(IPropertyAccessFodder1), "Property", 2);
            Assert.IsTrue(constraint.Eval(o), "Interface1 True");
            constraint = Property.Value(typeof(IPropertyAccessFodder1), "Property", 0);
            Assert.IsFalse(constraint.Eval(o), "Interface1 False");

            constraint = Property.Value(typeof(IPropertyAccessFodder2), "Property", null);
            Assert.IsTrue(constraint.Eval(o), "Interface2 True");
            constraint = Property.Value(typeof(IPropertyAccessFodder2), "Property", 0);
            Assert.IsFalse(constraint.Eval(o), "Interface2 False");

            constraint = Property.Value(typeof(DerivedPropertyAccessFodder), "Property", "4");
            Assert.IsTrue(constraint.Eval(o), "Derived True");
            constraint = Property.Value(typeof(DerivedPropertyAccessFodder), "Property", "0");
            Assert.IsFalse(constraint.Eval(o), "Derived False");

            // Also test that we can use the disambiguation to access private properties
            constraint = Property.Value(typeof(BasePropertyAccessFodder), "PrivateProperty", 5);
            Assert.IsTrue(constraint.Eval(o), "BasePrivate True");
            constraint = Property.Value(typeof(BasePropertyAccessFodder), "PrivateProperty", 0);
            Assert.IsFalse(constraint.Eval(o), "BasePrivate False");

            constraint = Property.Value(typeof(DerivedPropertyAccessFodder), "PrivateProperty", 6);
            Assert.IsTrue(constraint.Eval(o), "DerivedPrivate True");
            constraint = Property.Value(typeof(DerivedPropertyAccessFodder), "PrivateProperty", 0);
            Assert.IsFalse(constraint.Eval(o), "DerivedPrivate False");
        }

        [Test(Description = "Tests that Property.ValueConstraint() can disambiguate between properties of the same name")]
        public void DisambiguatedPropertyConstraintAccess()
        {
            DerivedPropertyAccessFodder o = new DerivedPropertyAccessFodder();

            AbstractConstraint constraint;

            constraint = Property.ValueConstraint(typeof(BasePropertyAccessFodder), "Property", Is.Equal(1));
            Assert.IsTrue(constraint.Eval(o), "Base True");
            constraint = Property.ValueConstraint(typeof(BasePropertyAccessFodder), "Property", Is.Equal(0));
            Assert.IsFalse(constraint.Eval(o), "Base False");

            constraint = Property.ValueConstraint(typeof(IPropertyAccessFodder1), "Property", Is.Equal(2));
            Assert.IsTrue(constraint.Eval(o), "Interface1 True");
            constraint = Property.ValueConstraint(typeof(IPropertyAccessFodder1), "Property", Is.Equal(0));
            Assert.IsFalse(constraint.Eval(o), "Interface1 False");

            constraint = Property.ValueConstraint(typeof(IPropertyAccessFodder2), "Property", Is.Equal(null));
            Assert.IsTrue(constraint.Eval(o), "Interface2 True");
            constraint = Property.ValueConstraint(typeof(IPropertyAccessFodder2), "Property", Is.Equal(0));
            Assert.IsFalse(constraint.Eval(o), "Interface2 False");

            constraint = Property.ValueConstraint(typeof(DerivedPropertyAccessFodder), "Property", Is.Equal("4"));
            Assert.IsTrue(constraint.Eval(o), "Derived True");
            constraint = Property.ValueConstraint(typeof(DerivedPropertyAccessFodder), "Property", Is.Equal("0"));
            Assert.IsFalse(constraint.Eval(o), "Derived False");

            // Also test that we can use the disambiguation to access private properties
            constraint = Property.ValueConstraint(typeof(BasePropertyAccessFodder), "PrivateProperty", Is.Equal(5));
            Assert.IsTrue(constraint.Eval(o), "BasePrivate True");
            constraint = Property.ValueConstraint(typeof(BasePropertyAccessFodder), "PrivateProperty", Is.Equal(0));
            Assert.IsFalse(constraint.Eval(o), "BasePrivate False");

            constraint = Property.ValueConstraint(typeof(DerivedPropertyAccessFodder), "PrivateProperty", Is.Equal(6));
            Assert.IsTrue(constraint.Eval(o), "DerivedPrivate True");
            constraint = Property.ValueConstraint(typeof(DerivedPropertyAccessFodder), "PrivateProperty", Is.Equal(0));
            Assert.IsFalse(constraint.Eval(o), "DerivedPrivate False");
        }

        [Test(Description = "Tests that Property.IsNull() can disambiguate between properties of the same name")]
        public void DisambiguatedPropertyIsNullAccess()
        {
            DerivedPropertyAccessFodder o = new DerivedPropertyAccessFodder();

            AbstractConstraint constraint;

            constraint = Property.IsNull(typeof(BasePropertyAccessFodder), "Property");
            Assert.IsFalse(constraint.Eval(o), "Base False");

            constraint = Property.IsNull(typeof(IPropertyAccessFodder2), "Property");
            Assert.IsTrue(constraint.Eval(o), "Interface2 True");
        }

        [Test(Description = "Tests that Property.IsNotNull() can disambiguate between properties of the same name")]
        public void DisambiguatedPropertyIsNotNullAccess()
        {
            DerivedPropertyAccessFodder o = new DerivedPropertyAccessFodder();

            AbstractConstraint constraint;

            constraint = Property.IsNotNull(typeof(BasePropertyAccessFodder), "Property");
            Assert.IsTrue(constraint.Eval(o), "Base True");

            constraint = Property.IsNotNull(typeof(IPropertyAccessFodder2), "Property");
            Assert.IsFalse(constraint.Eval(o), "Interface2 False");
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
    }
}
