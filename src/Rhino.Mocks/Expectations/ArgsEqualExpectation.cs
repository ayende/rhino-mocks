using System;
using System.Reflection;
using System.Text;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Expectations
{
	/// <summary>
	/// Summary description for ArgsEqualExpectation.
	/// </summary>
	public class ArgsEqualExpectation : AbstractExpectation
	{
		private readonly object[] expectedArgs;

		/// <summary>
		/// Creates a new <see cref="ArgsEqualExpectation"/> instance.
		/// </summary>
		/// <param name="expectedArgs">Expected args.</param>
		/// <param name="method">method this expectation is for</param>
		public ArgsEqualExpectation(MethodInfo method, object[] expectedArgs) : base(method)
		{
            this.expectedArgs = expectedArgs;
		}

		/// <summary>
		/// Validate the arguments for the method.
		/// </summary>
		/// <param name="args">The arguments with which the method was called</param>
		protected override bool DoIsExpected(object[] args)
		{
			return Validate.ArgsEqual(expectedArgs, args);
		}

		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value></value>
		public override string ErrorMessage
		{
            get
            {
				MethodCallUtil.FormatArgumnet format = new MethodCallUtil.FormatArgumnet(FormatArg);
				string methodCall = MethodCallUtil.StringPresentation(format, Method, ExpectedArgs);
				return base.CreateErrorMessage(methodCall);
            }
		}

		/// <summary>
		/// Get the expected args.
		/// </summary>
		public object[] ExpectedArgs
		{
			get { return expectedArgs; }
		}

		/// <summary>
		/// Determines if the object equal to expectation
		/// </summary>
		public override bool Equals(object obj)
		{
			ArgsEqualExpectation other = obj as ArgsEqualExpectation;
			if (other == null)
				return false;
            return Method.Equals(other.Method) && Validate.ArgsEqual(expectedArgs, other.expectedArgs);
		}

		/// <summary>
		/// Get the hash code
		/// </summary>
		public override int GetHashCode()
		{
			return Method.GetHashCode();
		}


		private static string FormatArg(Array args, int i)
		{
			if (args.Length <= i)
				return "missing parameter";
			object arg = args.GetValue(i);
			if (arg is Array)
			{
				Array arr = (Array)arg;
				StringBuilder sb = new StringBuilder();
				sb.Append('[');
				for (int j = 0; j < arr.Length; j++)
				{
					sb.Append(FormatArg(arr, j));
					if (j < arr.Length - 1)
						sb.Append(", ");
				}
				sb.Append("]");
				return sb.ToString();
			}
			else if (arg is string)
			{
				return '"' + arg.ToString() + '"';
			}
			else if (arg == null)
			{
				return "null";
			}
			else
			{
				return arg.ToString();
			}
		}
	}
}