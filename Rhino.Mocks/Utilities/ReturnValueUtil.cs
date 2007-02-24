using System;
using Castle.Core.Interceptor;

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
		/// Note that we need to get the value even for opened generic types, such as those from
		/// generic methods.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="invocation">The invocation.</param>
		/// <returns>the default value</returns>
		public static object DefaultValue(Type type, IInvocation invocation)
		{
			type = GenericsUtil.GetRealType(type, invocation);
			if (type.IsValueType == false || type==typeof(void))
				return null;
			return Activator.CreateInstance(type);
		}
	}
}