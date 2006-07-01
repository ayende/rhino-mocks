using Rhino.Mocks.Constraints;

namespace Rhino.Mocks
{
	/*
	 * class: Text
	 * 
	 * Contraints to deal with text and strings
	 * 
	 */ 
	/// <summary>
	/// Central location for all text related constraints
	/// </summary>
	public
#if dotNet2
    static
#else
    sealed
#endif
        class Text
	{
		/*
		 * Method: StartsWith
		 * 
		 * The parameter starts with the specified string
		 */ 
		/// <summary>
		/// Constrain the argument to starts with the specified string
		/// </summary>
		public static AbstractConstraint StartsWith(string start)
		{
			return new StartsWith(start);
		}

		/*
		 * Method: EndsWith
		 * 
		 * The parameter ends with the specified string
		 */ 
		/// <summary>
		/// Constrain the argument to end with the specified string
		/// </summary>
		public static AbstractConstraint EndsWith(string end)
		{
			return new EndsWith(end);
		}


		/*
		 * Method: Contains
		 * 
		 * The parameter contains the specified string
		 */ 
		/// <summary>
		/// Constrain the argument to contain the specified string
		/// </summary>
		public static AbstractConstraint Contains(string innerString)
		{
			return new Contains(innerString);
		}

	   /*
		* Method: Like
		* 
		* The parameter must satisfied the specified regular expression.
		*/ 
		/// <summary>
		/// Constrain the argument to validate according to regex pattern
		/// </summary>
		public static AbstractConstraint Like(string pattern)
		{
			return new Like(pattern);
		}
	}
}