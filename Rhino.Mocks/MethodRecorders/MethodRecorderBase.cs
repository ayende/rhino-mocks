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
using System.Diagnostics;
using System.Reflection;
using Castle.DynamicProxy;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Generated;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using System.Collections;

namespace Rhino.Mocks.MethodRecorders
{
	/// <summary>
	/// Base class for method recorders, handle delegating to inner recorder if needed.
	/// </summary>
	public abstract class MethodRecorderBase : IMethodRecorder
	{
		/// <summary>
		/// List of the expected actions on for this recorder
		/// The legal values are:
		///		* Expectations
		///		* Method Recorders
		/// </summary>
		protected IList recordedActions = new ArrayList();

		/// <summary>
		/// The current recorder.
		/// </summary>
		protected IMethodRecorder recorderToCall;

		/// <summary>
		/// The current replayer;
		/// </summary>
		protected IMethodRecorder replayerToCall;

		/// <summary>
		/// The parent recorder of this one, may be null.
		/// </summary>
		protected IMethodRecorder parentRecorder;

		/// <summary>
		/// This contains a list of all the replayers that should be ignored
		/// for a spesific method call. A replayer gets into this list by calling 
		/// ClearReplayerToCall() on its parent. This list is Clear()ed on each new invocation.
		/// </summary>
		private readonly IList replayersToIgnoreForThisCall = new ArrayList();

		/// <summary>
		/// All the repeatable methods calls.
		/// </summary>
		private ProxyMethodExpectationsDictionary repeatableMethods;

		/// <summary>
		/// Counts the recursion depth of the current expectation search stack
		/// </summary>
		private int recursionDepth = 0;

		/// <summary>
		/// Creates a new <see cref="MethodRecorderBase"/> instance.
		/// </summary>
		protected MethodRecorderBase(ProxyMethodExpectationsDictionary repeatableMethods)
		{
			this.repeatableMethods = repeatableMethods;
			recorderToCall = null;
			replayerToCall = null;
		}

		/// <summary>
		/// Creates a new <see cref="MethodRecorderBase"/> instance.
		/// </summary>
		/// <param name="parentRecorder">Parent recorder.</param>
		/// <param name="repeatableMethods">Repeatable methods</param>
		protected MethodRecorderBase(IMethodRecorder parentRecorder, ProxyMethodExpectationsDictionary repeatableMethods)
			: this(repeatableMethods)
		{
			this.parentRecorder = parentRecorder;
		}

		/// <summary>
		/// Records the specified call with the specified args on the mocked object.
		/// </summary>
		public void Record(object proxy, MethodInfo method, IExpectation expectation)
		{
			if (recorderToCall != null)
				recorderToCall.Record(proxy, method, expectation);
			else
				DoRecord(proxy, method, expectation);
		}

		/// <summary>
		/// Get the expectation for this method on this object with this arguments 
		/// </summary>
		public IExpectation GetRecordedExpectation(IInvocation invocation, object proxy, MethodInfo method, object[] args)
		{
			Validate.IsNotNull(proxy, "proxy");
			Validate.IsNotNull(method, "method");
			Validate.IsNotNull(args, "args");
			if (replayerToCall != null)
				return replayerToCall.GetRecordedExpectation(invocation, proxy, method, args);

			//merge recorders that contains only a single empty recorder
			if (recordedActions.Count == 1 && recordedActions[0] is IMethodRecorder)
			{
				replayerToCall = (IMethodRecorder)recordedActions[0];
				return replayerToCall.GetRecordedExpectation(invocation, proxy, method, args);
			}

			IExpectation expectation = DoGetRecordedExpectation(invocation, proxy, method, args);
			if (HasExpectations == false)
				MoveToParentReplayer();
			return expectation;

		}

		/// <summary>
		/// Gets the all expectations for a mocked object and method combination,
		/// regardless of the expected arguments / callbacks / contraints.
		/// </summary>
		/// <param name="proxy">Mocked object.</param>
		/// <param name="method">Method.</param>
		/// <returns>List of all relevant expectation</returns>
		public abstract ExpectationsList GetAllExpectationsForProxyAndMethod(object proxy, MethodInfo method);

		/// <summary>
		/// Gets the all expectations for proxy.
		/// </summary>
		/// <param name="proxy">Mocked object.</param>
		/// <returns>List of all relevant expectation</returns>
		public ExpectationsList GetAllExpectationsForProxy(object proxy)
		{
			ExpectationsList fromChild = null, mine;
			if (replayerToCall != null)
				fromChild = replayerToCall.GetAllExpectationsForProxy(proxy);
			mine = DoGetAllExpectationsForProxy(proxy);
			if (fromChild != null)
			{
				foreach (IExpectation expectation in fromChild)
				{
					if (mine.Contains(expectation) == false)
						mine.Add(expectation);
				}
			}
			return mine;
		}

		/// <summary>
		/// Replaces the old expectation with the new expectation for the specified proxy/method pair.
		/// This replace ALL expectations that equal to old expectations.
		/// </summary>
		/// <param name="proxy">Proxy.</param>
		/// <param name="method">Method.</param>
		/// <param name="oldExpectation">Old expectation.</param>
		/// <param name="newExpectation">New expectation.</param>
		public void ReplaceExpectation(object proxy, MethodInfo method, IExpectation oldExpectation, IExpectation newExpectation)
		{
			if (TryReplaceRepeatableExpectation(method, newExpectation, oldExpectation, proxy))
				return;
			if (recorderToCall != null)
				recorderToCall.ReplaceExpectation(proxy, method, oldExpectation, newExpectation);
			else
				DoReplaceExpectation(proxy, method, oldExpectation, newExpectation);
		}

		private bool TryReplaceRepeatableExpectation(MethodInfo method, IExpectation newExpectation, IExpectation oldExpectation, object proxy)
		{
			ProxyMethodPair pair = new ProxyMethodPair(proxy, method);
			if (repeatableMethods.ContainsKey(pair))
			{
				ExpectationsList expectationsList = repeatableMethods[pair];
				int indexOf = expectationsList.IndexOf(oldExpectation);
				if (indexOf != -1)
				{
					expectationsList[indexOf] = newExpectation;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Remove the all repeatable expectations for proxy.
		/// </summary>
		/// <param name="proxy">Mocked object.</param>
		public void RemoveAllRepeatableExpectationsForProxy(object proxy)
		{
			ProxyMethodPair[] keys = new ProxyMethodPair[repeatableMethods.Keys.Count];
			repeatableMethods.Keys.CopyTo(keys, 0);
			foreach (ProxyMethodPair pair in keys)
			{
				if (MockedObjectsEquality.Instance.Equals(pair.Proxy, proxy))
					repeatableMethods.Remove(pair);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has expectations that weren't satisfied yet.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has expectations; otherwise, <c>false</c>.
		/// </value>
		public bool HasExpectations
		{
			get
			{
				if (replayerToCall != null)
					return replayerToCall.HasExpectations;
				return DoHasExpectations;
			}
		}


		/// <summary>
		/// Set the expectation so it can repeat any number of times.
		/// </summary>
		public void AddToRepeatableMethods(object proxy, MethodInfo method, IExpectation expectation)
		{
			//Just to get an error if I make a mistake and try to add a normal
			//method to the speical repeat method
			Debug.Assert(expectation.RepeatableOption != RepeatableOption.Normal);
			RemoveExpectation(expectation);
			ProxyMethodPair pair = new ProxyMethodPair(proxy, method);
			if (repeatableMethods.ContainsKey(pair) == false)
				repeatableMethods.Add(pair, new ExpectationsList());
			ExpectationsList expectationsList = repeatableMethods[pair];
			ExpectationNotOnList(expectationsList, expectation,
				MockRepository.IsStub(proxy));
			expectationsList.Add(expectation);
		}

		/// <summary>
		/// Removes the expectation from the recorder
		/// </summary>
		public void RemoveExpectation(IExpectation expectation)
		{
			if (recorderToCall != null)
				recorderToCall.RemoveExpectation(expectation);
			else
				DoRemoveExpectation(expectation);
		}

		/// <summary>
		/// Adds the recorder and turn it into the active recorder.
		/// </summary>
		/// <param name="recorder">Recorder.</param>
		public void AddRecorder(IMethodRecorder recorder)
		{
			if (recorderToCall != null)
				recorderToCall.AddRecorder(recorder);
			else
			{
				DoAddRecorder(recorder);
				recorderToCall = recorder;
			}
		}

		/// <summary>
		/// Moves to previous recorder.
		/// </summary>
		public bool MoveToPreviousRecorder()
		{
			if (recorderToCall == null)
				return true;
			if (recorderToCall.MoveToPreviousRecorder())
				recorderToCall = null;
			return false;
		}

		/// <summary>
		/// Moves to parent recorder.
		/// </summary>
		public void MoveToParentReplayer()
		{
			replayerToCall = null;
			if (parentRecorder == null || HasExpectations)
				return;
			parentRecorder.MoveToParentReplayer();
		}

		/// <summary>
		/// Gets the recorded expectation or null.
		/// </summary>
		public IExpectation GetRecordedExpectationOrNull(object proxy, MethodInfo method, object[] args)
		{
			recursionDepth += 1;
			try
			{
				if (replayerToCall != null)
					return replayerToCall.GetRecordedExpectationOrNull(proxy, method, args);
				else
					return DoGetRecordedExpectationOrNull(proxy, method, args);
			}
			finally
			{
				recursionDepth -= 1;
				if (recursionDepth == 0)
					replayersToIgnoreForThisCall.Clear();
			}
		}

		/// <summary>
		/// Clear the replayer to call (and all its chain of replayers).
		/// This also removes it from the list of expectations, so it will never be considered again
		/// </summary>
		public void ClearReplayerToCall(IMethodRecorder childReplayer)
		{
			replayerToCall = null;
			//recordedActions.Remove(childReplayer);
			replayersToIgnoreForThisCall.Add(childReplayer);
		}

		/// <summary>
		/// Get the expectation for this method on this object with this arguments 
		/// </summary>
		public abstract ExpectationViolationException UnexpectedMethodCall(IInvocation invocation, object proxy, MethodInfo method, object[] args);

		/// <summary>
		/// Gets the next expected calls string.
		/// </summary>
		public abstract string GetExpectedCallsMessage();

		#region Protected Methods


		/// <summary>
		/// Handles the real getting of the recorded expectation or null.
		/// </summary>
		protected abstract IExpectation DoGetRecordedExpectationOrNull(object proxy, MethodInfo method, object[] args);

		/// <summary>
		/// Handle the real execution of this method for the derived class
		/// </summary>
		protected abstract void DoRecord(object proxy, MethodInfo method, IExpectation expectation);

		/// <summary>
		/// Handle the real execution of this method for the derived class
		/// </summary>
		protected abstract IExpectation DoGetRecordedExpectation(IInvocation invocation, object proxy, MethodInfo method, object[] args);

		/// <summary>
		/// Handle the real execution of this method for the derived class
		/// </summary>
		protected abstract ExpectationsList DoGetAllExpectationsForProxy(object proxy);

		/// <summary>
		/// Handle the real execution of this method for the derived class
		/// </summary>
		protected abstract void DoReplaceExpectation(object proxy, MethodInfo method, IExpectation oldExpectation, IExpectation newExpectation);

		/// <summary>
		/// Handle the real execution of this method for the derived class
		/// </summary>
		protected abstract bool DoHasExpectations { get; }

		/// <summary>
		/// Handle the real execution of this method for the derived class
		/// </summary>
		protected abstract void DoRemoveExpectation(IExpectation expectation);


		/// <summary>
		/// Handle the real execution of this method for the derived class
		/// </summary>
		protected abstract void DoAddRecorder(IMethodRecorder recorder);

		#endregion

		/// <summary>
		/// Should this replayer be considered valid for this call?
		/// </summary>
		protected bool ShouldConsiderThisReplayer(IMethodRecorder replayer)
		{
			return replayersToIgnoreForThisCall.Contains(replayer) == false;
		}

		/// <summary>
		/// This check the methods that were setup using the SetupResult.For()
		/// or LastCall.Repeat.Any() and that bypass the whole expectation model.
		/// </summary>
		public IExpectation GetRepeatableExpectation(object proxy, MethodInfo method, object[] args)
		{
			ProxyMethodPair pair = new ProxyMethodPair(proxy, method);
			if (repeatableMethods.ContainsKey(pair) == false)
				return null;
			ExpectationsList list = repeatableMethods[pair];
			foreach (IExpectation expectation in list)
			{
				if (expectation.IsExpected(args))
				{
					expectation.AddActualCall();
					if (expectation.RepeatableOption == RepeatableOption.Never)
					{
						string errMsg = string.Format("{0} Expected #{1}, Actual #{2}.", expectation.ErrorMessage, expectation.Expected, expectation.ActualCallsCount);
						ExpectationViolationException exception = new ExpectationViolationException(errMsg);
						MockRepository.SetExceptionToBeThrownOnVerify(proxy, exception);
						throw exception;
					}
					return expectation;
				}
			}
			return null;
		}

		private void ExpectationNotOnList(ExpectationsList list, IExpectation expectation, bool isStub)
		{
			bool expectationExists = list.Contains(expectation);
			if (expectationExists == false)
				return;
			bool isProeprty = expectation.Method.IsSpecialName &&
							  (expectation.Method.Name.StartsWith("get_") ||
								expectation.Method.Name.StartsWith("set_"));
			if (isStub == false || isProeprty == false)
				throw new InvalidOperationException("The result for " + expectation.ErrorMessage + " has already been setup.");
			throw new InvalidOperationException("The result for " + expectation.ErrorMessage + " has already been setup. Properties are already stubbed with PropertyBehavior by default, no action is required");
		}
	}
}