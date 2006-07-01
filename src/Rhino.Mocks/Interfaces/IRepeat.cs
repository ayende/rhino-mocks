namespace Rhino.Mocks.Interfaces
{
	/*
	 * interface: IRepeat
	 * 
	 * Allows to specified the number of times a method is called.
	 */ 
	/// <summary>
	/// Allows to specify the number of time for method calls
	/// </summary>
	public interface IRepeat
	{
		/*
		 * Method: Twice
		 * 
		 * The method will repeat twice
		 */ 
		/// <summary>
		/// Repeat the method twice.
		/// </summary>
		IMethodOptions Twice();

		/*
		 * Method: Once
		 * 
		 * The method will repeat once.
		 * 
		 * note:
		 * This is the default behaviour. 
		 */ 
		/// <summary>
		/// Repeat the method once.
		/// </summary>
		IMethodOptions Once();

	    
	    /// <summary>
        /// Repeat the method at least once, then repeat as many time as it would like.
        /// </summary>
        IMethodOptions AtLeastOnce();
	    
		/*
		 * Method: Any
		 * 
		 * Repeat the method any number of times.
		 * 
		 * note:
		 * This has special affects in that this method would now ignore orderring.
		 */ 
		/// <summary>
		/// Repeat the method any number of times.
		/// This has special affects in that this method would now ignore orderring.
		/// </summary>
		IMethodOptions Any();

		/*
		 * Method: Times
		 * 
		 * Repeat the method the specified number of time between min & max.
		 * 
		 * Params:
		 *	- min - The minimum number of times the method can repeat
		 *  - max - The maximum number of times the method can repeat
		 * 
		 */ 
		/// <summary>
		/// Set the range to repeat an action.
		/// </summary>
		/// <param name="min">Min.</param>
		/// <param name="max">Max.</param>
		IMethodOptions Times(int min, int max);

		/*
		 * Method: Times
		 * 
		 * Set the exact number of times this method can repeat.
		 */ 
		/// <summary>
		/// Set the amount of times to repeat an action.
		/// </summary>
		IMethodOptions Times(int count);

		/*
		 * Method: Never
		 * 
		 * This method must not appear in the replay state.
		 * 
		 * note:
		 * This has special affects in that this method would now ignore orderring.
		 */ 
		/// <summary>
		/// This method must not appear in the replay state.
		/// This has special affects in that this method would now ignore orderring.
		/// </summary>
		IMethodOptions Never();
	}
}