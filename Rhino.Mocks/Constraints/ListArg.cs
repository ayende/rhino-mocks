using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Rhino.Mocks.Constraints
{
	/// <summary>
	/// Provides access to the constraints defined in the class <see cref="Text"/> to be used in context
	/// with the <see cref="Arg&lt;T&gt;"/> syntax.
	/// </summary>
	public class ListArg<T> // where T : IEnumerable
	{
		internal ListArg() { ;}

		/// <summary>
		/// Determines whether the specified object is in the parameter.
		/// The parameter must be IEnumerable.
		/// </summary>
		/// <param name="obj">Obj.</param>
		/// <returns></returns>
		public T IsIn(object obj)
		{
			ArgManager.AddInArgument(List.IsIn(obj));
			return default(T);
		}

		/// <summary>
		/// Determines whatever the parameter is in the collection.
		/// </summary>
		public T OneOf(IEnumerable collection)
		{
			ArgManager.AddInArgument(List.OneOf(collection));
			return default(T);
		}

		/// <summary>
		/// Determines that the parameter collection is identical to the specified collection
		/// </summary>
		public T Equal(IEnumerable collection)
		{
			ArgManager.AddInArgument(List.Equal(collection));
			return default(T);
		}

		/// <summary>
		/// Determines that the parameter collection has the specified number of elements.
		/// </summary>
		/// <param name="constraint">The constraint that should be applied to the collection count.</param>
		public T Count(AbstractConstraint constraint)
		{
			ArgManager.AddInArgument(List.Count(constraint));
			return default(T);
		}

		/// <summary>
		/// Determines that an element of the parameter collections conforms to another AbstractConstraint.
		/// </summary>
		/// <param name="index">The zero-based index of the list element.</param>
		/// <param name="constraint">The constraint which should be applied to the list element.</param>
		public T Element(int index, AbstractConstraint constraint)
		{
			ArgManager.AddInArgument(List.Element(index, constraint));
			return default(T);
		}

		///<summary>
		/// Determines that all elements of the specified collection are in the the parameter collection 
		///</summary>
		///<param name="collection">The collection to compare against</param>
		///<returns>The constraint which should be applied to the list parameter.</returns>
		public T ContainsAll(IEnumerable collection)
		{
			ArgManager.AddInArgument(List.ContainsAll(collection));
			return default(T);
		}

		/// <summary>
		/// Throws NotSupportedException. Don't use Equals to define constraints. Use Equal instead.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			throw new InvalidOperationException("Don't use Equals() to define constraints, use Equal() instead");
		}


		/* implement GetHashCode to avoid compiler warning */
		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
