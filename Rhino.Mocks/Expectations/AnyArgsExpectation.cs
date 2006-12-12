using System;
using System.Reflection;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Expectations
{
	/// <summary>
	/// Expectation that matchs any arguments for the method.
	/// </summary>
	public class AnyArgsExpectation : AbstractExpectation
	{
		/// <summary>
		/// Creates a new <see cref="AnyArgsExpectation"/> instance.
		/// </summary>
		/// <param name="method">Method.</param>
		public AnyArgsExpectation(MethodInfo method) : base(method)
		{
		}

		/// <summary>
		/// Creates a new <see cref="AnyArgsExpectation"/> instance.
		/// </summary>
		/// <param name="expectation">Expectation.</param>
		public AnyArgsExpectation(IExpectation expectation) : base(expectation)
		{
		}

		/// <summary>
		/// Validate the arguments for the method.
		/// </summary>
		/// <param name="args">The arguments with which the method was called</param>
		protected override bool DoIsExpected(object[] args)
		{
			return true;
		}

		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value></value>
		public override string ErrorMessage
		{
			get
			{
				MethodCallUtil.FormatArgumnet format = new MethodCallUtil.FormatArgumnet(AnyFormatArg);
				string stringPresentation = MethodCallUtil.StringPresentation(format, Method, new object[0]);
				return CreateErrorMessage(stringPresentation);
			}
		}

		/// <summary>
		/// Determines if the object equal to expectation
		/// </summary>
		public override bool Equals(object obj)
		{
			AnyArgsExpectation other = obj as AnyArgsExpectation;
			if (other == null)
				return false;
            return Method.Equals(other.Method);
		}

		/// <summary>
		/// Get the hash code
		/// </summary>
		public override int GetHashCode()
		{
			return Method.GetHashCode();
		}

		private  string AnyFormatArg(Array args, int i)
		{
			return "any";
		}
	}
}