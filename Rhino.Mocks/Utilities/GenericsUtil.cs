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
using System.Collections.Generic;
using Castle.DynamicProxy;

namespace Rhino.Mocks.Utilities
{
	/// <summary>
	/// Utility class for dealing with messing generics scenarios.
	/// </summary>
	public static class GenericsUtil
	{
		/// <summary>
		/// There are issues with trying to get this to work correctly with open generic types, since this is an edge case, 
		/// I am letting the runtime handle it.
		/// </summary>
		public static bool HasOpenGenericParam(Type returnType)
		{
			//not bound to particular type, only way I know of doing this, since IsGeneric and IsGenericTypeDefination will both lie
			//when used with generic method parameters
			if (returnType.FullName == null)
				return true;
			foreach (Type genericArgument in returnType.GetGenericArguments())
			{
				if (genericArgument.FullName == null)
					return true;
				if (genericArgument.IsGenericType)
				{
					if (HasOpenGenericParam(genericArgument))
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Gets the real type, including de-constructing and constructing the type of generic
		/// methods parameters.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="invocation">The invocation.</param>
		/// <returns></returns>
		public static Type GetRealType(Type type, IInvocation invocation)
		{
			if (!HasOpenGenericParam(type))
				return type;
			Dictionary<string, Type> nameToType = CreateTypesTableFromInvocation(invocation);
			string typeName = type.AssemblyQualifiedName ?? type.Name; // if the AssemblyQualifiedName is null, we have an open type
			if (nameToType.ContainsKey(typeName))
				return nameToType[typeName];
			Type[] types = new List<Type>(nameToType.Values).ToArray();
			return ReconstructGenericType(type, nameToType);
		}

		/// <summary>
		/// Because we need to support complex types here (simple generics were handled above) we
		/// need to be aware of the following scenarios:
		/// List[T] and List[Foo[T]]
		/// </summary>
		private static Type ReconstructGenericType(Type type, Dictionary<string, Type> nameToType)
		{
			Type genericTypeDef = type.GetGenericTypeDefinition();
			List<Type> genericArgs = new List<Type>();
			foreach (Type genericArgument in type.GetGenericArguments())
			{
				if(nameToType.ContainsKey(genericArgument.Name))
				{
					genericArgs.Add(nameToType[genericArgument.Name]);
				}
				else
				{
					genericArgs.Add( ReconstructGenericType(genericArgument, nameToType));
				}
			}
			return genericTypeDef.MakeGenericType(genericArgs.ToArray());
		}

		private static Dictionary<string, Type> CreateTypesTableFromInvocation(IInvocation invocation)
		{
			Dictionary<string, Type> nameToType = new Dictionary<string, Type>();
			Type genericType = GetTypeWithGenericArgumentsForMethodParameters(invocation);
			Type[] genericArguments = genericType.GetGenericTypeDefinition().GetGenericArguments();
			Type[] types = genericType.GetGenericArguments();
			for (int i = 0; i < genericArguments.Length; i++)
			{
				string genericName = genericArguments[i].Name;
				nameToType[genericName] = types[i];
			}
			return nameToType;
		}

		private static Type GetTypeWithGenericArgumentsForMethodParameters(IInvocation invocation)
		{
			Type genericType = invocation.GetType();
			if (genericType.IsGenericType) //generic method
				return genericType;
			//Generic class:

			Type type = MockRepository.GetMockedObject(invocation.Proxy).GetDeclaringType(invocation.Method);
			if (type == null)
				throw new InvalidOperationException("BUG: Could not find a declaring type for method " + invocation.Method);
			return type;
		}

		
	}
}