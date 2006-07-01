using System;
using System.Diagnostics;
using System.Reflection;
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
		private IList replayerToIgnoreForThisCall = new ArrayList();
		
		/// <summary>
		/// All the repeatable methods calls.
		/// </summary>
		private ProxyMethodExpectationsDictionary repeatableMethods;

		/// <summary>
		/// Creates a new <see cref="MethodRecorderBase"/> instance.
		/// </summary>
		public MethodRecorderBase()
		{
			repeatableMethods = new ProxyMethodExpectationsDictionary();
			recorderToCall = null;
			replayerToCall = null;
		}

		/// <summary>
		/// Creates a new <see cref="MethodRecorderBase"/> instance.
		/// </summary>
		/// <param name="parentRecorder">Parent recorder.</param>
		public MethodRecorderBase(IMethodRecorder parentRecorder) : this()
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
		public IExpectation GetRecordedExpectation(object proxy, MethodInfo method, object[] args)
		{
			Validate.IsNotNull(proxy, "proxy");
			Validate.IsNotNull(method, "method");
			Validate.IsNotNull(args, "args");
			if (replayerToCall != null)
				return replayerToCall.GetRecordedExpectation(proxy, method, args);
			IExpectation expectation = DoGetRecordedExpectation(proxy, method, args);
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
			if(fromChild!=null)
			{
				foreach (IExpectation expectation in fromChild)
				{
					if(mine.Contains(expectation)==false)
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
	            if(indexOf!=-1)
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
            repeatableMethods.Keys.CopyTo(keys,0);
            foreach (ProxyMethodPair pair in keys)
            {
                if (MockedObjectsEquality.Instance.Equals( pair.Proxy , proxy))
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
			ExpectationNotOnList(expectationsList, expectation);
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
			try
			{
				if (replayerToCall != null)
					return replayerToCall.GetRecordedExpectationOrNull(proxy, method, args);
				else
					return DoGetRecordedExpectationOrNull(proxy, method, args);
			}
        	finally
			{
				replayerToIgnoreForThisCall.Clear();
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
			replayerToIgnoreForThisCall.Add(childReplayer);
		}

		/// <summary>
		/// Get the expectation for this method on this object with this arguments 
		/// </summary>
		public abstract ExpectationViolationException UnexpectedMethodCall(object proxy, MethodInfo method, object[] args);
		
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
		protected abstract IExpectation DoGetRecordedExpectation(object proxy, MethodInfo method, object[] args);

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
			return replayerToIgnoreForThisCall.Contains(replayer) == false;
		}
		
		/// <summary>
		/// This check the methods that were setup using the SetupResult.For()
		/// or LastCall.Repeat.Any() and that bypass the whole expectation model.
		/// </summary>
		public IExpectation GetRepeatableExpectation(object proxy, MethodInfo method, object[] args)
		{
			ProxyMethodPair pair = new ProxyMethodPair(proxy, method);
            if (repeatableMethods.ContainsKey(pair)==false)
                return null;
			ExpectationsList list = repeatableMethods[pair];
			foreach (IExpectation expectation in list)
			{
				if (expectation.IsExpected(args))
				{
					expectation.AddActualCall();
					if (expectation.RepeatableOption == RepeatableOption.Never)
					{
						string errMsg = string.Format("{0} Expected #{1}, Actual #{2}.", expectation.ErrorMessage, expectation.Expected, expectation.ActualCalls);
						throw new ExpectationViolationException(errMsg);
					}
					return expectation;
				}
			}
            return null;
		}

		private void ExpectationNotOnList(ExpectationsList list, IExpectation expectation)
		{
			if (list.Contains(expectation))
				throw new InvalidOperationException("The result for " + expectation.ErrorMessage + " has already been setup.");
		}
    }
}