using System;
using System.Reflection;
using System.Text;
using Castle.Core.Interceptor;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Expectations
{
	/// <summary>
	/// Call a specified callback to verify the expectation
	/// </summary>
	public class CallbackExpectation : AbstractExpectation
	{
		private Delegate callback;

		/// <summary>
		/// Creates a new <see cref="CallbackExpectation"/> instance.
		/// </summary>
		/// <param name="expectation">Expectation.</param>
		/// <param name="callback">Callback.</param>
		public CallbackExpectation(IExpectation expectation, Delegate callback) : base(expectation)
		{
			this.callback = callback;
			ValidateCallback();
		}

		/// <summary>
		/// Creates a new <see cref="CallbackExpectation"/> instance.
		/// </summary>
		/// <param name="invocation">Invocation for this expectation</param>
		/// <param name="callback">Callback.</param>
		public CallbackExpectation(IInvocation invocation, Delegate callback) : base(invocation)
		{
			this.callback = callback;
			ValidateCallback();
		}

		/// <summary>
		/// Validate the arguments for the method on the child methods
		/// </summary>
		/// <param name="args">The arguments with which the method was called</param>
		protected override bool DoIsExpected(object[] args)
		{
			try
			{
				return (bool) callback.DynamicInvoke(args);
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}

		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value></value>
		public override string ErrorMessage
		{
			get
			{
				StringBuilder sb = new StringBuilder();
                sb.Append(Method.DeclaringType.Name).Append(".").Append(Method.Name);
				sb.Append("(").Append("callback method: ").Append(callback.Method.DeclaringType.Name);
				sb.Append(".").Append(callback.Method.Name).Append(");");
				return CreateErrorMessage(sb.ToString());
			}
		}

		private void ValidateCallback()
		{
			if (callback.Method.ReturnType != typeof (bool))
				throw new InvalidOperationException("Callbacks must return a boolean");
            AssertDelegateArgumentsMatchMethod(callback);
		}

		/// <summary>
		/// Determines if the object equal to expectation
		/// </summary>
		public override bool Equals(object obj)
		{
			CallbackExpectation other = obj as CallbackExpectation;
			if (other == null)
				return false;
            return Method.Equals(other.Method) && callback.Equals(other.callback);
		}

		/// <summary>
		/// Get the hash code
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}