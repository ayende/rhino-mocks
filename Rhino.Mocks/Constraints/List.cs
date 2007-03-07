using System.Collections;
using Rhino.Mocks.Constraints;

namespace Rhino.Mocks.Constraints
{
	/*
	 * class: List
	 * 
	 * Constraints for dealing with lists.
	 * 
	 */ 
	/// <summary>
	/// Central location for constraints about lists and collections
	/// </summary>
	public
#if dotNet2
 static
#else
    sealed
#endif
        class List
	{

		/*
		 * Method: IsIn
		 * 
		 * Determines whether the specified obj is in the paramter.
		 * The parameter must be IEnumerable.
		 */ 
		/// <summary>
		/// Determines whether the specified obj is in the paramter.
		/// The parameter must be IEnumerable.
		/// </summary>
		/// <param name="obj">Obj.</param>
		/// <returns></returns>
		public static AbstractConstraint IsIn(object obj)
		{
			return new IsIn(obj);
		}

		/*
		 * Method: OneOf
		 * 
		 * Determains whatever the parameter is in the collection.
		 */ 
		/// <summary>
		/// Determains whatever the parameter is in the collection.
		/// </summary>
		public static AbstractConstraint OneOf(ICollection collection)
		{
			return new OneOf(collection);
		}

		/*
		 * Method Equal
		 * Determains that the parameter collection is identical to the specified collection
		 * This is done by iterating the collections and comparing each element.
		 */ 
		/// <summary>
		/// Determains that the parameter collection is identical to the specified collection
		/// </summary>
		public static AbstractConstraint Equal(ICollection collection)
		{
			return new CollectionEqual(collection);
		}
	}
}