using System;
using System.Collections.Generic;
using Castle.Core.Interceptor;

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
			Type genericTypeDef = type.GetGenericTypeDefinition();
			Type[] types = new List<Type>(nameToType.Values).ToArray();
			return genericTypeDef.MakeGenericType(types);
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

			//Dynamic proxy is generating a proxy to the CLOSED generic object, need to 
			//find the type that is matching to the method declaring type.

			Type typeDeclaringTheMethod = invocation.Method.DeclaringType;
			Type mockedType = genericType.DeclaringType;
			if (IsMatch(mockedType, typeDeclaringTheMethod))
				return mockedType;
			foreach (Type mockedInterface in mockedType.GetInterfaces())
			{
				if (IsMatch(mockedInterface, typeDeclaringTheMethod))
					return mockedInterface;
			}
			throw new InvalidOperationException("BUG: Could not find the type defining parameters for the method " + invocation.Method);
		}

		private static bool IsMatch(Type mockedType, Type typeDeclaringTheMethod)
		{
			bool matched = mockedType == typeDeclaringTheMethod || typeDeclaringTheMethod == mockedType.GetGenericTypeDefinition();
			return matched;
		}
	}
}