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


using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Utilities;
using Rhino.Mocks.Impl;

namespace Rhino.Mocks.MethodRecorders
{
	using Generated;

	/// <summary>
	/// Ordered collection of methods, methods must arrive in specified order
	/// in order to pass.
	/// </summary>
	public class OrderedMethodRecorder : UnorderedMethodRecorder
	{
		/// <summary>
		/// Creates a new <see cref="OrderedMethodRecorder"/> instance.
		/// </summary>
		/// <param name="parentRecorder">Parent recorder.</param>
		/// <param name="repeatableMethods">Repetable methods</param>
		public OrderedMethodRecorder(IMethodRecorder parentRecorder, ProxyMethodExpectationsDictionary repeatableMethods)
			: base(parentRecorder, repeatableMethods)
		{
		}

		/// <summary>
		/// Creates a new <see cref="OrderedMethodRecorder"/> instance.
		/// </summary>
		public OrderedMethodRecorder(ProxyMethodExpectationsDictionary repeatableMethods)
			: base(repeatableMethods)
		{
		}

		/// <summary>
		/// Handles the real getting of the recorded expectation or null.
		/// </summary>
		protected override IExpectation DoGetRecordedExpectationOrNull(object proxy, MethodInfo method, object[] args)
		{
			int actionPos = 0;
			while (actionPos < recordedActions.Count)
			{
				ProxyMethodExpectationTriplet triplet = recordedActions[actionPos] as ProxyMethodExpectationTriplet;
				if (triplet != null)
				{
					if (MockedObjectsEquality.Instance.Equals(triplet.Proxy, proxy) &&
						triplet.Method == method &&
						triplet.Expectation.CanAcceptCalls &&
						triplet.Expectation.IsExpected(args))
					{
						// Ensure that this expectation is the first one in the list.
						while (actionPos > 0)
						{
							recordedActions.RemoveAt(0);
							actionPos--;
						}

						// This call satisfies that expectation.
						triplet.Expectation.AddActualCall();

						// Is this expectation complete?
						if (!triplet.Expectation.CanAcceptCalls)
						{
							recordedActions.RemoveAt(0);
						}

						return triplet.Expectation;
					}
					else
					{
						// The expectation didn't match.  Is the expectation satisfied, so can we consider
						// looking at the next expectation?
						if (triplet.Expectation.ExpectationSatisfied)
						{
							actionPos++;
						}
						else
						{
							// No.
							return null;
						}
					}
				}
				else // Action is another recorder
				{
					IMethodRecorder innerRecorder = (IMethodRecorder)recordedActions[actionPos];
					if (innerRecorder.HasExpectations == false) // so don't need to consider it
					{
						actionPos++;
						continue;
					}
					if (ShouldConsiderThisReplayer(innerRecorder) == false)
						break;
					IExpectation expectation = innerRecorder.GetRecordedExpectationOrNull(proxy, method, args);
					if (expectation != null)
					{
						// Ensure that this expectation is the first one in the list.
						while (actionPos > 0)
						{
							recordedActions.RemoveAt(0);
							actionPos--;
						}

						replayerToCall = innerRecorder;
						recordedActions.RemoveAt(0);
						return expectation;
					}
					break;
				}
			}
			if (parentRecorder == null)
				return null;
			// We only reach this place if we still has valid expectations, but they are not
			// mandatory, (AtLeastOnce(), etc). In this case, the recorder (and its children) cannot satisfy the 
			// expectation, so we move to the parent recorder and let it handle it.
			parentRecorder.ClearReplayerToCall(this);
			// We need this to give the correct exception if the method is an unepxected one.
			// Check the redirection in UnexpectedMethodCall()
			parentRecorderRedirection = parentRecorder;
			return parentRecorder.GetRecordedExpectationOrNull(proxy, method, args);
		}

		/// <summary>
		/// Get the expectation for this method on this object with this arguments 
		/// </summary>
		public override ExpectationViolationException UnexpectedMethodCall(IInvocation invocation, object proxy, MethodInfo method, object[] args)
		{
			// We have move to the parent recorder, we need to pass the call to it.
			if (parentRecorderRedirection != null)
				return parentRecorderRedirection.UnexpectedMethodCall(invocation, proxy, method, args);
			StringBuilder sb = new StringBuilder();
			sb.Append("Unordered method call! The expected call is: '");
			sb.Append(GetExpectedCallsMessage());
			sb.Append("' but was: '").
				Append(MethodCallUtil.StringPresentation(invocation, method, args)).
				Append("'");
			return new ExpectationViolationException(sb.ToString());
		}

		/// <summary>
		/// Gets the next expected calls string.
		/// </summary>
		public override string GetExpectedCallsMessage()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Ordered: { ");
			if (recordedActions.Count > 0)
			{
				ProxyMethodExpectationTriplet triplet = recordedActions[0] as ProxyMethodExpectationTriplet;
				if (triplet != null)
				{
					sb.Append(triplet.Expectation.ErrorMessage);
				}
				else //Action is another recorder
				{
					sb.Append(((IMethodRecorder)recordedActions[0]).GetExpectedCallsMessage());
				}
			}
			else
			{
				sb.Append("No method call is expected");
			}
			sb.Append(" }");
			return sb.ToString();
		}
	}
}
