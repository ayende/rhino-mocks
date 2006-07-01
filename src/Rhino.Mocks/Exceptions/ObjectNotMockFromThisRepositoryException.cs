using System;
using System.Runtime.Serialization;

namespace Rhino.Mocks.Exceptions
{
	/*
	 * Class: ObjectNotMockFromThisRepositoryException
	 * Signals that an object was call on a mock repostiroy which doesn't
	 * belong to this mock repository or not a mock
	 */ 
	/// <summary>
	/// Signals that an object was call on a mock repostiroy which doesn't
	/// belong to this mock repository or not a mock
	/// </summary>
	[Serializable()]
	public class ObjectNotMockFromThisRepositoryException : Exception
	{
		#region Constructors

		/// <summary>
		/// Creates a new <see cref="ObjectNotMockFromThisRepositoryException"/> instance.
		/// </summary>
		/// <param name="message">Message.</param>
		public ObjectNotMockFromThisRepositoryException(string message) : base(message)
		{
		}
		
		/// <summary>
		/// Serialization constructor
		/// </summary>
		protected ObjectNotMockFromThisRepositoryException(SerializationInfo info, StreamingContext context)
        :base(info,context){} 

		#endregion
	}
}