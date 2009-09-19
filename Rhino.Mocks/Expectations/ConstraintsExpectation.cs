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
using Castle.Core.Interceptor;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Expectations
{
	/// <summary>
	/// Expect the method's arguments to match the contraints
	/// </summary>
	public class ConstraintsExpectation : AbstractExpectation
	{
		private AbstractConstraint[] constraints;

		/// <summary>
		/// Creates a new <see cref="ConstraintsExpectation"/> instance.
		/// </summary>
		/// <param name="invocation">Invocation for this expectation</param>
		/// <param name="constraints">Constraints.</param>
        /// <param name="expectedRange">Number of method calls for this expectations</param>
		public ConstraintsExpectation(IInvocation invocation,AbstractConstraint[] constraints, Range expectedRange) : base(invocation, expectedRange)
		{
			Validate.IsNotNull(constraints, "constraints");
			this.constraints = constraints;
			ConstraintsMatchMethod();
		}

		/// <summary>
		/// Creates a new <see cref="ConstraintsExpectation"/> instance.
		/// </summary>
		/// <param name="expectation">Expectation.</param>
		/// <param name="constraints">Constraints.</param>
		public ConstraintsExpectation(IExpectation expectation, AbstractConstraint[] constraints) : base(expectation)
		{
			Validate.IsNotNull(constraints, "constraints");
			this.constraints = constraints;
			ConstraintsMatchMethod();
		}

		/// <summary>
		/// Validate the arguments for the method.
		/// </summary>
		/// <param name="args">The arguments with which the method was called</param>
		protected override bool DoIsExpected(object[] args)
		{
			Validate.IsNotNull(args, "args");
			if (args.Length != constraints.Length)
				throw new InvalidOperationException("Number of argument doesn't match the number of parameters!");
			for (int i = 0; i < args.Length; i++)
			{
				if (constraints[i].Eval(args[i]) == false)
					return false;
			}
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
				MethodCallUtil.FormatArgumnet format = new MethodCallUtil.FormatArgumnet(FormatArgWithConstraint);
				string stringPresentation = MethodCallUtil.StringPresentation(Originalinvocation, format, Method, constraints);
				return CreateErrorMessage(stringPresentation);
			}
		}

		private void ConstraintsMatchMethod()
		{
            if (constraints.Length != Method.GetParameters().Length)
				throw new InvalidOperationException("The number of constraints is not the same as the number of the method's parameters!");
			for (int i = 0; i < constraints.Length; i++)
			{
				if (constraints[i] == null)
					throw new InvalidOperationException(string.Format("The constraint at index {0} is null! Use Is.Null() to represent null parameters.", i));
			}
		}

		private string FormatArgWithConstraint(Array args, int i)
		{
			return constraints[i].Message;
		}

		/// <summary>
		/// Determines if the object equal to expectation
		/// </summary>
		public override bool Equals(object obj)
		{
			ConstraintsExpectation other = obj as ConstraintsExpectation;
			if (other == null)
				return false;
            return Method.Equals(other.Method) && Validate.ArgsEqual(constraints, other.constraints);
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
