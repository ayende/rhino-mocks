using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Rhino.Mocks.Constraints
{
    internal class AllPropertiesMatchConstraint : AbstractConstraint
    {
        private object _expected; //object holding the expected property values.
        private string _message; //the message that Rhino Mocks will show.
        private Stack<string> _properties; //used to build the property name like Order.Product.Price for the message.
        private List<object> _checkedObjects; //every object that is matched goes in this list to prevent recursive loops.

        /// <summary>
        /// Initializes a new constraint object.
        /// </summary>
        /// <param name="expected">The expected object, The actual object is passed in as a parameter to the <see cref="Eval"/> method</param>
        public AllPropertiesMatchConstraint(object expected)
        {
            _expected = expected;
            _properties = new Stack<string>();
            _checkedObjects = new List<object>();
        }

        /// <summary>
        /// Evaluate this constraint.
        /// </summary>
        /// <param name="obj">The actual object that was passed in the method call to the mock.</param>
        /// <returns>True when the constraint is met, else false.</returns>
        public override bool Eval(object obj)
        {
            _properties.Clear();
            _checkedObjects.Clear();
            _properties.Push(obj.GetType().Name);
            bool result = CheckReferenceType(_expected, obj);
            _properties.Pop();
            _checkedObjects.Clear();
            return result;
        }

        /// <summary>
        /// Rhino.Mocks uses this property to generate an error message.
        /// </summary>
        /// <value>
        /// A message telling the tester why the constraint failed.
        /// </value>
        public override string Message
        {
            get { return _message; }
        }


        /// <summary>
        /// Checks if the properties of the <paramref name="actual"/> object
        /// are the same as the properies of the <paramref name="expected"/> object.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <returns>True when both objects have the same values, else False.</returns>
        protected virtual bool CheckReferenceType(object expected, object actual)
        {
            Type tExpected = expected.GetType();
            Type tActual = actual.GetType();


            if (tExpected != tActual)
            {
                _message = string.Format("Expected type '{0}' doesn't match with actual type '{1}'", tExpected.Name, tActual.Name);
                return false;
            }

            return CheckValue(expected, actual);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        /// <remarks>This is the real heart of the beast.</remarks>
        protected virtual bool CheckValue(object expected, object actual)
        {
            if (actual == null && expected != null)
            {
                _message = string.Format("Expected value of {0} is '{1}', actual value is null", BuildPropertyName(), expected);
                return false;
            }
            
            if (expected == null)
            {
                if (actual != null)
                {
                    _message = string.Format("Expected value of {0} is null, actual value is '{1}'", BuildPropertyName(), actual);
                    return false;
                }
            }
            else
            {
                //if both objects are comparable Equals can be used to determine equality. (value types implement IComparable too when boxed)
                if (expected is IComparable) 
                {
                    if (!expected.Equals(actual))
                    {
                        _message = string.Format("Expected value of {0} is '{1}', actual value is '{2}'", BuildPropertyName(), expected.ToString(), actual.ToString());
                        return false;
                    }
                }
                else if (expected is IEnumerable) //if both objects are lists we should tread them as such.
                {
                    if (!CheckCollection((IEnumerable)expected, (IEnumerable)actual))
                    {
                        return false;
                    }
                }
                else if (!_checkedObjects.Contains(expected)) //prevent endless recursive loops.
                {
                    _checkedObjects.Add(expected);
                    if (!CheckProperties(expected, actual))
                    {
                        return false;
                    }
                    if (!CheckFields(expected, actual))
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// Used by CheckReferenceType to check all properties of the reference type.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <returns>True when both objects have the same values, else False.</returns>
        protected virtual bool CheckProperties(object expected, object actual)
        {
            Type tExpected = expected.GetType();
            Type tActual = actual.GetType();

            PropertyInfo[] properties = tExpected.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo property in properties)
            {
                //TODO: deal with indexed properties
                ParameterInfo[] indexParameters = property.GetIndexParameters();
                if (indexParameters.Length == 0) //It's not an indexed property
                {
                    _properties.Push(property.Name);

                    try
                    {
                        object expectedValue = property.GetValue(expected, null);
                        object actualValue = property.GetValue(actual, null);

                        //if (!CheckValue(expectedValue, actualValue)) return false;
                        if (!CheckValue(expectedValue, actualValue)) return false;
                    }
                    catch (System.Reflection.TargetInvocationException)
                    {
                        //the inner exception should give you a clou about why we couldn't invoke GetValue...
                        //do nothing
                    }

                    _properties.Pop();
                }
            }
            return true;
        }


        /// <summary>
        /// Used by CheckReferenceType to check all fields of the reference type.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <returns>True when both objects have the same values, else False.</returns>
        protected virtual bool CheckFields(object expected, object actual)
        {
            Type tExpected = expected.GetType();
            Type tActual = actual.GetType();
            
            _checkedObjects.Add(actual);

            FieldInfo[] fields = tExpected.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                _properties.Push(field.Name);

                object expectedValue = field.GetValue(expected);
                object actualValue = field.GetValue(actual);

                bool result = CheckValue(expectedValue, actualValue);
                _properties.Pop();
                if (!result) return false;
            }

            return true;
        }


        /// <summary>
        /// Checks the items of both collections
        /// </summary>
        /// <param name="expectedCollection">The expected collection</param>
        /// <param name="actualCollection"></param>
        /// <returns>True if both collections contain the same items in the same order.</returns>
        private bool CheckCollection(IEnumerable expectedCollection, IEnumerable actualCollection)
        {
            if (expectedCollection != null) //only check the list if there is something in there.
            {
                IEnumerator expectedEnumerator = expectedCollection.GetEnumerator();
                IEnumerator actualEnumerator = actualCollection.GetEnumerator();
                bool expectedHasMore = expectedEnumerator.MoveNext();
                bool actualHasMore = actualEnumerator.MoveNext();
                int expectedCount = 0;
                int actualCount = 0;
                string name = _properties.Pop(); //pop the propertyname from the stack to be replaced by the same name with an index.

                while (expectedHasMore && actualHasMore)
                {
                    object expectedValue = expectedEnumerator.Current;
                    object actualValue = actualEnumerator.Current;

                    _properties.Push(name + string.Format("[{0}]", expectedCount)); //replace the earlier popped property name
                    
                    expectedCount++;
                    actualCount++;

                    if (!CheckReferenceType(expectedValue, actualValue))
                    {
                        return false;
                    }

                    _properties.Pop(); //pop the old indexed property name to make place for a new one

                    expectedHasMore = expectedEnumerator.MoveNext();
                    actualHasMore = actualEnumerator.MoveNext();
                }
                _properties.Push(name); //push the original property name back on the stack.

                //examine the expectedMoveNextResult and the actualMoveNextResult to see if one collection was bigger than the other.
                if (expectedHasMore & !actualHasMore) //actual has less items than expected.
                {
                    //find out how much items there are in the expected collection.
                    do expectedCount++; while (expectedEnumerator.MoveNext());
                }
                if (!expectedHasMore & actualHasMore) //actual has more items than expected.
                {
                    //find out how much items there are in the actual collection.
                    do actualCount++; while (actualEnumerator.MoveNext());
                }
                if (expectedCount != actualCount)
                {
                    _message = string.Format("expected number of items in collection {0} is '{1}', actual is '{2}'", BuildPropertyName(), expectedCount, actualCount);
                    return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Builds a propertyname from the Stack _properties like 'Order.Product.Price'
        /// to be used in the error message.
        /// </summary>
        /// <returns>A nested property name.</returns>
        private string BuildPropertyName()
        {
            StringBuilder result = new StringBuilder();
            string[] names = _properties.ToArray();
            foreach(string name in names)
            {
                if (result.Length > 0)
                {
                    result.Insert(0, '.');
                }
                result.Insert(0, name);
            }
            return result.ToString();
        }
    }
}
