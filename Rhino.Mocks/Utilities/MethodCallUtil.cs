using System;
using System.Reflection;
using System.Text;
using Castle.Core.Interceptor;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Utilities
{
	/// <summary>
	/// Utility class for working with method calls.
	/// </summary>
	public
#if dotNet2
		static
#else
        sealed
#endif
		class MethodCallUtil
	{
		/// <summary>
		/// Delegate to format the argument for the string representation of
		/// the method call.
		/// </summary>
		public delegate string FormatArgumnet(Array args, int i);

		/// <summary>
		/// Return the string representation of a method call and its arguments.
		/// </summary>
		/// <param name="method">The method</param>
		/// <param name="args">The method arguments</param>
		/// <param name="invocation">Invocation of the method, used to get the generics arguments</param>
		/// <param name="format">Delegate to format the parameter</param>
		/// <returns>The string representation of this method call</returns>
		public static string StringPresentation(IInvocation invocation, FormatArgumnet format, MethodInfo method, object[] args)
		{
			Validate.IsNotNull(format, "format");
			Validate.IsNotNull(method, "method");
			Validate.IsNotNull(args, "args");
			StringBuilder sb = new StringBuilder();
			sb.Append(method.DeclaringType.Name).Append(".").Append(method.Name);
#if dotNet2
			if (invocation != null)
			{
				if (method.IsGenericMethod)
				{
					sb.Append("<");
					foreach (Type genericArgument in invocation.GetType().GetGenericArguments())
					{
						sb.Append(genericArgument);
						sb.Append(", ");
					}
					sb.Remove(sb.Length - 2, 2); //remove last ", " 
					sb.Append(">");
				}
			}

#endif
			sb.Append("(");
			int numberOfParameters = method.GetParameters().Length;
			for (int i = 0; i < numberOfParameters; i++)
			{
				sb.Append(format(args, i));
				if (i < numberOfParameters - 1)
					sb.Append(", ");
			}
			sb.Append(");");
			return sb.ToString();
		}

		/// <summary>
		/// Return the string representation of a method call and its arguments.
		/// </summary>
		/// <param name="invocation">The invocation of the method, used to get the generic parameters</param>
		/// <param name="method">The method</param>
		/// <param name="args">The method arguments</param>
		/// <returns>The string representation of this method call</returns>
		public static string StringPresentation(IInvocation invocation, MethodInfo method, object[] args)
		{
			return StringPresentation(invocation, new FormatArgumnet(DefaultFormatArgument), method, args);
		}

		#region Private Methods

		private static string DefaultFormatArgument(Array args, int i)
		{
			if (args.Length <= i)
				return "missing parameter";
			object arg = args.GetValue(i);
			if (arg is Array)
			{
				Array arr = (Array) arg;
				StringBuilder sb = new StringBuilder();
				sb.Append('[');
				for (int j = 0; j < arr.Length; j++)
				{
					sb.Append(DefaultFormatArgument(arr, j));
					if (j < arr.Length - 1)
						sb.Append(", ");
				}
				sb.Append("]");
				return sb.ToString();
			}
			if (arg is string)
				return "\"" + arg.ToString() + "\"";
			else if (arg == null)
				return "null";
			else
				return MockingSafeToString(arg);
		}

		// we need to ensure that we won't re-eenterant into the repository
		// if the parameter is a mock object
		private static string MockingSafeToString(object arg)
		{
			IMockedObject mock = arg as IMockedObject;
			if(mock==null)
				return arg.ToString();
			return mock.GetType().BaseType.FullName;
		}

		#endregion
	}
}