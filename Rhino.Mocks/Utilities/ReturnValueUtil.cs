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


using System;
using System.Collections;
using System.Collections.Generic;
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
		/// Empty Collections for IEnumerable, ICollection, IList and IDictionary types
		/// Null for the rest of reference types and void
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
				return TryInstantiateCollectionInterfacesOrReturnNull(type);

			return Activator.CreateInstance(type);
		}

		private static object TryInstantiateCollectionInterfacesOrReturnNull(Type returnType)
		{
			if (!returnType.IsInterface) return null;

			if (returnType.IsGenericType)
			{
				Type[] arguments = returnType.GetGenericArguments();

				Type typeDefinition = returnType.GetGenericTypeDefinition();

				if (typeDefinition == typeof(IList<>) ||
				    typeDefinition == typeof(IEnumerable<>) ||
				    typeDefinition == typeof(ICollection<>))
					return Activator.CreateInstance(typeof(List<>).MakeGenericType(arguments));

				if (typeDefinition == typeof(IDictionary<,>))
					return Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(arguments));

				return null;
			}
			if (returnType == (typeof(IList)) ||
				returnType == typeof(IEnumerable) ||
				returnType.Equals(typeof(ICollection)))
				return new ArrayList();

			if (returnType.Equals(typeof(IDictionary)))
				return new Dictionary<object, object>();

			return null;
		}
	}
}