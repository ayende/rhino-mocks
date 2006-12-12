using System;
using System.Reflection;
using System.Text;
using Rhino.Mocks.Impl;

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
		/// <param name="format">Delegate to format the parameter</param>
		/// <returns>The string representation of this method call</returns>
		public static string StringPresentation(FormatArgumnet format, MethodInfo method, object[] args)
		{
			Validate.IsNotNull(format, "format");
			Validate.IsNotNull(method, "method");
			Validate.IsNotNull(args, "args");
			StringBuilder sb = new StringBuilder();
			sb.Append(method.DeclaringType.Name).Append(".").Append(method.Name);
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
		/// <param name="method">The method</param>
		/// <param name="args">The method arguments</param>
		/// <returns>The string representation of this method call</returns>
		public static string StringPresentation(MethodInfo method, object[] args)
		{
			return MethodCallUtil.StringPresentation(new FormatArgumnet(DefaultFormatArgument), method, args);
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
					sb.Append(DefaultFormatArgument( arr, j));
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
				return arg.ToString();
		}

		#endregion
	}
}