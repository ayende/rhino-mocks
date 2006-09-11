using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Constraints
{

    #region PropertyIs

    /// <summary>
    /// Constrain that the property has a specified value
    /// </summary>
    public class PropertyIs : PropertyConstraint
    {
        /// <summary>
        /// Creates a new <see cref="PropertyIs"/> instance.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="expectedValue">Expected value.</param>
        public PropertyIs(string propertyName, object expectedValue)
            : base(propertyName, Is.Equal(expectedValue))
        {
        }

        /// <summary>
        /// Creates a new <see cref="PropertyIs"/> instance, specifying a disambiguating
        /// <paramref name="declaringType"/> for the property.
        /// </summary>
        /// <param name="declaringType">The type that declares the property, used to disambiguate between properties.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="expectedValue">Expected value.</param>
        public PropertyIs(Type declaringType, string propertyName, object expectedValue)
            : base(declaringType, propertyName, Is.Equal(expectedValue))
        {
        }
    }

    #endregion

    #region PropertyConstraint

    /// <summary>
    /// Constrain that the property matches another constraint.
    /// </summary>
    public class PropertyConstraint : AbstractConstraint
    {
        private readonly Type declaringType;
        private readonly string propertyName;
        private readonly AbstractConstraint constraint;

        /// <summary>
        /// Creates a new <see cref="PropertyConstraint"/> instance.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="constraint">Constraint to place on the property value.</param>
        public PropertyConstraint(string propertyName, AbstractConstraint constraint)
            : this(null, propertyName, constraint)
        {
        }

        /// <summary>
        /// Creates a new <see cref="PropertyConstraint"/> instance, specifying a disambiguating
        /// <paramref name="declaringType"/> for the property.
        /// </summary>
        /// <param name="declaringType">The type that declares the property, used to disambiguate between properties.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="constraint">Constraint to place on the property value.</param>
        public PropertyConstraint(Type declaringType, string propertyName, AbstractConstraint constraint)
        {
            this.declaringType = declaringType;
            this.propertyName = propertyName;
            this.constraint = constraint;
        }

        /// <summary>
        /// Determines if the object passes the constraint.
        /// </summary>
        public override bool Eval(object obj)
        {
            if (obj == null)
                return false;
            PropertyInfo prop;

            if (declaringType == null)
            {
                prop = obj.GetType().GetProperty(propertyName);
            }
            else
            {
                prop = declaringType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            }
            if (prop == null || !prop.CanRead)
                return false;
            object propertyValue = prop.GetValue(obj, null);
            return constraint.Eval(propertyValue);
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get { return "property '" + propertyName + "' " + constraint.Message; }
        }
    }

    #endregion

    #region TypeOf

    /// <summary>
    /// Constrain that the parameter must be of the specified type
    /// </summary>
    public class TypeOf : AbstractConstraint
    {
        private Type type;

        /// <summary>
        /// Creates a new <see cref="TypeOf"/> instance.
        /// </summary>
        /// <param name="type">Type.</param>
        public TypeOf(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// determains if the object pass the constraints
        /// </summary>
        public override bool Eval(object obj)
        {
            return type.IsInstanceOfType(obj);
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get { return "type of {" + type.FullName + "}"; }
        }
    }

    #endregion

    #region Same
    /// <summary>
    /// Constraint that determines whether an object is the same object as another.
    /// </summary>
    public class Same : AbstractConstraint
    {
        private readonly object same;

        /// <summary>
        /// Creates a new <see cref="Equal"/> instance.
        /// </summary>
        /// <param name="obj">Obj.</param>
        public Same(object obj)
        {
            this.same = obj;
        }

        /// <summary>
        /// Determines if the object passes the constraints.
        /// </summary>
        public override bool Eval(object obj)
        {
            return Object.ReferenceEquals(same, obj);
        }

        /// <summary>
        /// Gets the message for this constraint.
        /// </summary>
        public override string Message
        {
            get
            {
                string sameAsString = (same == null) ? "null" : same.ToString();
                return "same as " + sameAsString;
            }
        }
    }
    #endregion

	#region Predicate Constraint
	
#if dotNet2
	
	/// <summary>
	/// Evaluate a parameter using constraints
	/// </summary>
	public class PredicateConstraint<T> : AbstractConstraint
	{
		Predicate<T> predicate;

		/// <summary>
		/// Create new instance 
		/// </summary>
		/// <param name="predicate"></param>
		public PredicateConstraint(Predicate<T> predicate)
		{
			Validate.IsNotNull(predicate, "predicate");
			this.predicate = predicate;
		}

		/// <summary>
		/// determains if the object pass the constraints
		/// </summary>
		public override bool Eval(object obj)
		{
			if(obj!=null && 
				obj.GetType().IsAssignableFrom(typeof(T))==false)
			{
				throw new InvalidOperationException(
					string.Format("Predicate accept {0} but parameter is {1} which is not compatible",
					              typeof (T).FullName,
					              obj.GetType().FullName));
			}
			return predicate((T) obj);
		}

		/// <summary>
		/// Gets the message for this constraint
		/// </summary>
		/// <value></value>
		public override string Message
		{
			get
			{
				return string.Format("Predicate ({0})", MethodCallUtil.StringPresentation(FormatEmptyArgumnet,predicate.Method, new object[0]));
			}
		}
		
		private string FormatEmptyArgumnet(Array args, int i)
		{
			return "obj";
		}
	}
	
	
#endif
	
	#endregion

	#region List constraints

	#region Equal

	/// <summary>
    /// Constrain that the list contains the same items as the parameter list
    /// </summary>
    public class CollectionEqual : AbstractConstraint
    {
        private ICollection collection;

        /// <summary>
        /// Creates a new <see cref="CollectionEqual"/> instance.
        /// </summary>
        /// <param name="collection">In list.</param>
        public CollectionEqual(ICollection collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// determains if the object pass the constraints
        /// </summary>
        public override bool Eval(object obj)
        {
            ICollection arg = obj as ICollection;
            if (arg != null)
            {
                if (arg.Count != collection.Count)
                    return false;
                IEnumerator argEnumerator = arg.GetEnumerator(),
                    collectionEnumerator = collection.GetEnumerator();
                while (argEnumerator.MoveNext() && collectionEnumerator.MoveNext())
                {
                    if (argEnumerator.Current.Equals(collectionEnumerator.Current) == false)
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("equal to collection [");
                int i = 0;
                foreach (object o in collection)
                {
                    sb.Append(o);
                    if (i < collection.Count - 1)
                        sb.Append(", ");
                    i++;
                }
                sb.Append("]");
                return sb.ToString();
            }
        }
    }

    #endregion

    #region OneOf

    /// <summary>
    /// Constrain that the parameter is one of the items in the list
    /// </summary>
    public class OneOf : AbstractConstraint
    {
        private ICollection collection;

        /// <summary>
        /// Creates a new <see cref="OneOf"/> instance.
        /// </summary>
        /// <param name="collection">In list.</param>
        public OneOf(ICollection collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// determains if the object pass the constraints
        /// </summary>
        public override bool Eval(object obj)
        {
            foreach (object o in collection)
            {
                if (obj.Equals(o))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("one of [");
                int i = 0;
                foreach (object o in collection)
                {
                    sb.Append(o);
                    if (i < collection.Count - 1)
                        sb.Append(", ");
                    i++;
                }
                sb.Append("]");
                return sb.ToString();
            }
        }
    }

    #endregion

    #region IsIn

    /// <summary>
    /// Constrain that the object is inside the parameter list
    /// </summary>
    public class IsIn : AbstractConstraint
    {
        private object inList;

        /// <summary>
        /// Creates a new <see cref="IsIn"/> instance.
        /// </summary>
        /// <param name="inList">In list.</param>
        public IsIn(object inList)
        {
            this.inList = inList;
        }

        /// <summary>
        /// determains if the object pass the constraints
        /// </summary>
        public override bool Eval(object obj)
        {
            if (obj is IEnumerable)
            {
                foreach (object o in (IEnumerable)obj)
                {
                    if (inList.Equals(o))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get { return "list contains [" + inList + "]"; }
        }
    }

    #endregion

    #endregion

    #region Logic Operator

    #region Or

    /// <summary>
    /// Combines two constraints, constraint pass if either is fine.
    /// </summary>
    public class Or : AbstractConstraint
    {
        private AbstractConstraint c1, c2;

        /// <summary>
        /// Creates a new <see cref="And"/> instance.
        /// </summary>
        /// <param name="c1">C1.</param>
        /// <param name="c2">C2.</param>
        public Or(AbstractConstraint c1, AbstractConstraint c2)
        {
            this.c1 = c1;
            this.c2 = c2;
        }

        /// <summary>
        /// determains if the object pass the constraints
        /// </summary>
        public override bool Eval(object obj)
        {
            return c1.Eval(obj) || c2.Eval(obj);
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get { return c1.Message + " or " + c2.Message; }
        }
    }

    #endregion

    #region Not

    /// <summary>
    /// Negate a constraint
    /// </summary>
    public class Not : AbstractConstraint
    {
        private AbstractConstraint c1;

        /// <summary>
        /// Creates a new <see cref="And"/> instance.
        /// </summary>
        /// <param name="c1">C1.</param>
        public Not(AbstractConstraint c1)
        {
            this.c1 = c1;
        }

        /// <summary>
        /// determains if the object pass the constraints
        /// </summary>
        public override bool Eval(object obj)
        {
            return !c1.Eval(obj);
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get { return "not " + c1.Message; }
        }
    }

    #endregion

    #region And

    /// <summary>
    /// Combines two constraints
    /// </summary>
    /// <remarks></remarks>
    public class And : AbstractConstraint
    {
        private AbstractConstraint c1, c2;

        /// <summary>
        /// Creates a new <see cref="And"/> instance.
        /// </summary>
        /// <param name="c1">C1.</param>
        /// <param name="c2">C2.</param>
        public And(AbstractConstraint c1, AbstractConstraint c2)
        {
            this.c1 = c1;
            this.c2 = c2;
        }

        /// <summary>
        /// determains if the object pass the constraints
        /// </summary>
        public override bool Eval(object obj)
        {
            return c1.Eval(obj) && c2.Eval(obj);
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get { return c1.Message + " and " + c2.Message; }
        }
    }

    #endregion

    #endregion

    #region String Constraints

    #region Like

    /// <summary>
    /// Constrain the argument to validate according to regex pattern
    /// </summary>
    public class Like : AbstractConstraint
    {
        private string pattern;
        private Regex regex;

        /// <summary>
        /// Creates a new <see cref="Like"/> instance.
        /// </summary>
        /// <param name="pattern">Pattern.</param>
        public Like(string pattern)
        {
            regex = new Regex(pattern);
            this.pattern = pattern;
        }

        /// <summary>
        /// determains if the object pass the constraints
        /// </summary>
        public override bool Eval(object obj)
        {
            if (obj is string)
            {
                return regex.IsMatch((string)obj);
            }
            return false;
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get { return "like \"" + pattern + "\""; }
        }
    }

    #endregion

    #region Contains

    /// <summary>
    /// Constraint that evaluate whatever an argument contains the specified string.
    /// </summary>
    public class Contains : AbstractConstraint
    {
        private string innerString;

        /// <summary>
        /// Creates a new <see cref="Contains"/> instance.
        /// </summary>
        /// <param name="innerString">Inner string.</param>
        public Contains(string innerString)
        {
            this.innerString = innerString;
        }

        /// <summary>
        /// determains if the object pass the constraints
        /// </summary>
        public override bool Eval(object obj)
        {
            if (obj is string)
                return ((string)obj).IndexOf(innerString) > -1;
            return false;
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get { return "contains \"" + innerString + "\""; }
        }
    }

    #endregion

    #region Ends With

    /// <summary>
    /// Constraint that evaluate whatever an argument ends with the specified string
    /// </summary>
    public class EndsWith : AbstractConstraint
    {
        private string end;

        /// <summary>
        /// Creates a new <see cref="EndsWith"/> instance.
        /// </summary>
        /// <param name="end">End.</param>
        public EndsWith(string end)
        {
            this.end = end;
        }

        /// <summary>
        /// determains if the object pass the constraints
        /// </summary>
        public override bool Eval(object obj)
        {
            if (obj is string)
                return ((string)obj).EndsWith(end);
            return false;
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get { return "ends with \"" + end + "\""; }
        }
    }

    #endregion

    #region Starts With

    /// <summary>
    /// Constraint that evaluate whatever an argument start with the specified string
    /// </summary>
    public class StartsWith : AbstractConstraint
    {
        private string start;

        /// <summary>
        /// Creates a new <see cref="StartsWith"/> instance.
        /// </summary>
        /// <param name="start">Start.</param>
        public StartsWith(string start)
        {
            this.start = start;
        }

        /// <summary>
        /// determains if the object pass the constraints
        /// </summary>
        public override bool Eval(object obj)
        {
            if (obj is string)
                return ((string)obj).StartsWith(start);
            return false;
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get { return "starts with \"" + start + "\""; }
        }
    }

    #endregion

    #endregion

    #region Object Constraints

    #region Equals

    /// <summary>
    /// Constraint that evaluate whatever an object equals another
    /// </summary>
    public class Equal : AbstractConstraint
    {
        private object equal;

        /// <summary>
        /// Creates a new <see cref="Equal"/> instance.
        /// </summary>
        /// <param name="obj">Obj.</param>
        public Equal(object obj)
        {
            this.equal = obj;
        }

        /// <summary>
        /// determains if the object pass the constraints
        /// </summary>
        public override bool Eval(object obj)
        {
            if (obj == null)
                return equal == null;
            return obj.Equals(equal);
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get
            {
                string equalAsString = equal == null ? "null" : equal.ToString();
                return "equal to " + equalAsString;
            }
        }
    }

    #endregion

    #region Anything

    /// <summary>
    /// Constraint that always returns true
    /// </summary>
    public class Anything : AbstractConstraint
    {
        /// <summary>
        /// determains if the object pass the constraints
        /// </summary>
        public override bool Eval(object obj)
        {
            return true;
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get { return "anything"; }
        }
    }

    #endregion

    #endregion

    #region Math Constraints

    /// <summary>
    /// Constraint that evaluate whatever a comparable is greater than another
    /// </summary>
    public class ComparingConstraint : AbstractConstraint
    {
        private IComparable compareTo;
        private readonly bool largerThan;
        private readonly bool andEqual;

        /// <summary>
        /// Creates a new <see cref="ComparingConstraint"/> instance.
        /// </summary>
        public ComparingConstraint(IComparable compareTo, bool largerThan, bool andEqual)
        {
            this.compareTo = compareTo;
            this.largerThan = largerThan;
            this.andEqual = andEqual;
        }

        /// <summary>
        /// determains if the object pass the constraints
        /// </summary>
        public override bool Eval(object obj)
        {
            if (obj is IComparable)
            {
                int result = ((IComparable)obj).CompareTo(compareTo);
                if (result == 0 && andEqual)
                    return true;
                if (largerThan)
                    return result > 0;
                else
                    return result < 0;
            }
            return false;
        }

        /// <summary>
        /// Gets the message for this constraint
        /// </summary>
        /// <value></value>
        public override string Message
        {
            get
            {
                string result;
                if (largerThan)
                    result = "greater than ";
                else
                    result = "less than ";
                if (andEqual)
                    result += "or equal to ";
                return result + compareTo;
            }
        }
    }

    #endregion
}
