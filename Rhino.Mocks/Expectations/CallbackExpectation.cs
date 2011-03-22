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
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Rhino.Mocks.Impl;
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
		/// <param name="expectedRange">Number of method calls for this expectations</param>
		public CallbackExpectation(IInvocation invocation, Delegate callback, Range expectedRange) : base(invocation, expectedRange)
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
