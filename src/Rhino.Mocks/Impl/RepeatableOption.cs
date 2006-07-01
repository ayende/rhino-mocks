using System;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Options for special repeat option
	/// </summary>
	public enum RepeatableOption
	{
		/// <summary>
		/// This method can be called only as many times as the IMethodOptions.Expect allows.
		/// </summary>
		Normal,
		/// <summary>
		/// This method should never be called
		/// </summary>
		Never,
		/// <summary>
		/// This method can be call any number of times
		/// </summary>
		Any,
		/// <summary>
		/// This method will call the original method
		/// </summary>
		OriginalCall,
        /// <summary>
        /// This method will simulate simple property behavior
        /// </summary>
        PropertyBehavior
	}
}
