using System;

namespace Rhino.Mocks.Utilities
{
	/// <summary>
	/// Utility to get the default value for a type
	/// </summary>
	public class ReturnValueUtil
	{
		/// <summary>
		/// The default value for a type.
		/// Null for reference types and void
		/// 0 for value types.
		/// First element for enums
		/// </summary>
		/// <param name="type">Type.</param>
		/// <returns>the default value</returns>
		public static object DefaultValue(Type type)
		{
			if (type.IsValueType == false || type==typeof(void))
				return null;
			return Activator.CreateInstance(type);
		}
	}
}