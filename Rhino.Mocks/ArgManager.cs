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
//using System.Linq;
using System.Text;
using System.Collections;
using Rhino.Mocks.Constraints;
using System.Reflection;

namespace Rhino.Mocks
{

	/// <summary>
	/// Used to manage the static state of the Arg&lt;T&gt; class"/>
	/// </summary>
	internal class ArgManager
	{
		[ThreadStatic]
		private static List<ArgumentDefinition> args;


		internal static bool HasBeenUsed 
		{ 
			get 
			{
				if (args == null) return false;
				return args.Count > 0; 
			} 
		}

		/// <summary>
		/// Resets the static state
		/// </summary>
		internal static void Clear()
		{
			args = new List<ArgumentDefinition>();
		}

		internal static void AddInArgument(AbstractConstraint constraint)
		{
			InitializeThreadStatic();
			args.Add(new ArgumentDefinition(constraint));
		}

		internal static void AddOutArgument(object returnValue)
		{
			InitializeThreadStatic();
			args.Add(new ArgumentDefinition(returnValue));
		}

		internal static void AddRefArgument(AbstractConstraint constraint, object returnValue)
		{
			InitializeThreadStatic();
			args.Add(new ArgumentDefinition(constraint, returnValue));
		}


		/// <summary>
		/// Returns return values for the out and ref parameters
		/// Note: the array returned has the size of the number of out and ref 
		/// argument definitions
		/// </summary>
		/// <returns></returns>
		internal static object[] GetAllReturnValues()
		{
			InitializeThreadStatic();
			List<object> returnValues = new List<object>();
			foreach (ArgumentDefinition arg in args)
			{
				if (arg.InOutRef == InOutRefArgument.OutArg || arg.InOutRef == InOutRefArgument.RefArg)
				{
					returnValues.Add(arg.returnValue);
				}
			}
			return returnValues.ToArray();
		}

		/// <summary>
		/// Returns the constraints for all arguments.
		/// Out arguments have an Is.Anything constraint and are also in the list.
		/// </summary>
		/// <returns></returns>
		internal static AbstractConstraint[] GetAllConstraints()
		{
			InitializeThreadStatic();
			List<AbstractConstraint> constraints = new List<AbstractConstraint>();
			foreach (ArgumentDefinition arg in args)
			{
				constraints.Add(arg.constraint);
			}
			return constraints.ToArray();
		}

		internal static void CheckMethodSignature(MethodInfo method)
		{
			InitializeThreadStatic();
			ParameterInfo[] parameters = method.GetParameters();
			AbstractConstraint[] constraints = new AbstractConstraint[parameters.Length];

			if (args.Count < parameters.Length)
			{
				throw new InvalidOperationException(
					string.Format("When using Arg<T>, all arguments must be defined using Arg<T>.Is, Arg<T>.Text, Arg<T>.List, Arg<T>.Ref or Arg<T>.Out. {0} arguments expected, {1} have been defined.",
						parameters.Length, args.Count));
			}
			if (args.Count > parameters.Length)
			{
				throw new InvalidOperationException(
					string.Format("Use Arg<T> ONLY within a mock method call while recording. {0} arguments expected, {1} have been defined.",
						parameters.Length, args.Count));
			}

			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].IsOut)
				{
					if (args[i].InOutRef != InOutRefArgument.OutArg)
					{
						throw new InvalidOperationException(
							string.Format("Argument {0} must be defined as: out Arg<T>.Out(returnvalue).Dummy",
								i));
					}
				}
				else if (parameters[i].ParameterType.IsByRef)
				{
					if (args[i].InOutRef != InOutRefArgument.RefArg)
					{
						throw new InvalidOperationException(
							string.Format("Argument {0} must be defined as: ref Arg<T>.Ref(constraint, returnvalue).Dummy",
								i));
					}
				}
				else if (args[i].InOutRef != InOutRefArgument.InArg)
				{
					throw new InvalidOperationException(
						string.Format("Argument {0} must be defined using: Arg<T>.Is, Arg<T>.Text or Arg<T>.List",
							i));
				}
			}
		}

		private static void InitializeThreadStatic()
		{
			if (args == null)
			{
				args = new List<ArgumentDefinition>();
			}
		}

		private struct ArgumentDefinition
		{
			public InOutRefArgument InOutRef;
			public AbstractConstraint constraint;
			public object returnValue;
			public ArgumentDefinition(AbstractConstraint constraint)
			{
				this.InOutRef = InOutRefArgument.InArg;
				this.constraint = constraint;
				this.returnValue = null;
			}
			public ArgumentDefinition(AbstractConstraint constraint, object returnValue)
			{
				this.InOutRef = InOutRefArgument.RefArg;
				this.constraint = constraint;
				this.returnValue = returnValue;
			}
			public ArgumentDefinition(object returnValue)
			{
				this.InOutRef = InOutRefArgument.OutArg;
				this.returnValue = returnValue;
				this.constraint = Is.Anything();
			}

		}

		private enum InOutRefArgument
		{
			InArg,
			OutArg,
			RefArg
		}

	}
}
