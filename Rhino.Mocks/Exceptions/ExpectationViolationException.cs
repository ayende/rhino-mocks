using System;
using System.Runtime.Serialization;

namespace Rhino.Mocks.Exceptions
{
	/*
	 * Class: ExpectationViolationException
	 * This exception is thrown when there is a an expectation violation.
	 */ 
	/// <summary>
	/// An expectaton violation was detected.
	/// </summary>
	[Serializable()]
	public class ExpectationViolationException : Exception
	{
		#region Constructors

		/// <summary>
		/// Creates a new <see cref="ExpectationViolationException"/> instance.
		/// </summary>
		/// <param name="message">Message.</param>
		public ExpectationViolationException(string message) : base(message)
		{
		}
		
		
		/// <summary>
		/// Serialization constructor
		/// </summary>
		protected ExpectationViolationException(SerializationInfo info, StreamingContext context)
        :base(info,context){} 

		#endregion
	}
}