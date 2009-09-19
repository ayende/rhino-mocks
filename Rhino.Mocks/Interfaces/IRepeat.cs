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
	public interface IRepeat<T>
	{
		/*
		 * Method: Twice
		 * 
		 * The method will repeat twice
		 */ 
		/// <summary>
		/// Repeat the method twice.
		/// </summary>
		IMethodOptions<T> Twice();

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
		IMethodOptions<T> Once();

	    
	    /// <summary>
        /// Repeat the method at least once, then repeat as many time as it would like.
        /// </summary>
        IMethodOptions<T> AtLeastOnce();
	    
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
		IMethodOptions<T> Any();

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
		IMethodOptions<T> Times(int min, int max);

		/*
		 * Method: Times
		 * 
		 * Set the exact number of times this method can repeat.
		 */ 
		/// <summary>
		/// Set the amount of times to repeat an action.
		/// </summary>
		IMethodOptions<T> Times(int count);

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
		IMethodOptions<T> Never();
	}
}