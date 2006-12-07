using System.Collections;
using System.Reflection;
using System.Text;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Generated;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.MethodRecorders
{
	/// <summary>
	/// Unordered collection of method records, any expectation that exist
	/// will be matched.
	/// </summary>
	public class UnorderedMethodRecorder : MethodRecorderBase
	{
		/// <summary>
		/// The parent recorder we have redirected to.
		/// Useful for certain edge cases in orderring.
		/// See: FieldProblem_Entropy for the details.
		/// </summary>
		protected IMethodRecorder parentRecorderRedirection = null;
		
		/// <summary>
		/// Creates a new <see cref="UnorderedMethodRecorder"/> instance.
		/// </summary>
		/// <param name="parentRecorder">Parent recorder.</param>
		public UnorderedMethodRecorder(IMethodRecorder parentRecorder) : base(parentRecorder)
		{
		}

		/// <summary>
		/// Creates a new <see cref="UnorderedMethodRecorder"/> instance.
		/// </summary>
		public UnorderedMethodRecorder()
		{
		}

		/// <summary>
		/// Records the specified call with the specified args on the mocked object.
		/// </summary>
		/// <param name="proxy">Mocked object.</param>
		/// <param name="method">Method.</param>
		/// <param name="expectation">Expectation.</param>
		protected override void DoRecord(object proxy, MethodInfo method, IExpectation expectation)
		{
			Validate.IsNotNull(proxy, "proxy");
			Validate.IsNotNull(method, "method");
			Validate.IsNotNull(expectation, "expectation");
			ProxyMethodExpectationTriplet entry = new ProxyMethodExpectationTriplet(proxy, method, expectation);
			recordedActions.Add(entry);
		}

		/// <summary>
		/// Get the expectation for this method on this object with this arguments 
		/// </summary>
		/// <param name="proxy">Mocked object.</param>
		/// <param name="method">Method.</param>
		/// <param name="args">Args.</param>
		/// <returns>True is the call was recorded, false otherwise</returns>
		protected override IExpectation DoGetRecordedExpectation(object proxy, MethodInfo method, object[] args)
		{
			IExpectation expectation = GetRecordedExpectationOrNull(proxy, method, args);
			if (expectation == null)
				throw UnexpectedMethodCall(proxy, method, args);
			return expectation;
		}

		/// <summary>
		/// Gets the all expectations for a mocked object and method combination,
		/// regardless of the expected arguments / callbacks / contraints.
		/// </summary>
		/// <param name="proxy">Mocked object.</param>
		/// <param name="method">Method.</param>
		/// <returns>List of all relevant expectation</returns>
		public override ExpectationsList GetAllExpectationsForProxyAndMethod(object proxy, MethodInfo method)
		{
			Validate.IsNotNull(proxy, "proxy");
			Validate.IsNotNull(method, "method");

			ExpectationsList expectations = new ExpectationsList();
			foreach (object action in recordedActions)
			{
				ProxyMethodExpectationTriplet triplet = action as ProxyMethodExpectationTriplet;
				if (triplet != null)
				{
					if (MockedObjectsEquality.Instance.Equals( triplet.Proxy , proxy ) &&
						triplet.Method == method)
					{
						expectations.Add(triplet.Expectation);
					}
				}
				else //Action is another recorder
				{
					IMethodRecorder innerRecorder = (IMethodRecorder) action;
					expectations.AddRange(innerRecorder.GetAllExpectationsForProxyAndMethod(proxy, method));
				}
			}
			return expectations;
		}

		/// <summary>
		/// Gets the all expectations for proxy.
		/// </summary>
		/// <param name="proxy">Mocked object.</param>
		/// <returns>List of all relevant expectation</returns>
		protected override ExpectationsList DoGetAllExpectationsForProxy(object proxy)
		{
			Validate.IsNotNull(proxy, "proxy");

			ExpectationsList expectations = new ExpectationsList();
			foreach (object action in recordedActions)
			{
				ProxyMethodExpectationTriplet triplet = action as ProxyMethodExpectationTriplet;
				if (triplet != null)
				{
					if (MockedObjectsEquality.Instance.Equals( triplet.Proxy , proxy))
					{
						expectations.Add(triplet.Expectation);
					}
				}
				else //Action is another recorder
				{
					IMethodRecorder innerRecorder = (IMethodRecorder) action;
					ExpectationsList expectationsForProxy = innerRecorder.GetAllExpectationsForProxy(proxy);
					expectations.AddRange(expectationsForProxy);
				}
			}
			return expectations;
		}

		/// <summary>
		/// Replaces the old expectation with the new expectation for the specified proxy/method pair.
		/// This replace ALL expectations that equal to old expectations.
		/// </summary>
		/// <param name="proxy">Proxy.</param>
		/// <param name="method">Method.</param>
		/// <param name="oldExpectation">Old expectation.</param>
		/// <param name="newExpectation">New expectation.</param>
		protected override void DoReplaceExpectation(object proxy, MethodInfo method, IExpectation oldExpectation, IExpectation newExpectation)
		{
			Validate.IsNotNull(proxy, "proxy");
			Validate.IsNotNull(method, "method");
			Validate.IsNotNull(oldExpectation, "oldExpectation");
			Validate.IsNotNull(newExpectation, "newExpectation");
			foreach (object action in recordedActions)
			{
				ProxyMethodExpectationTriplet triplet = action as ProxyMethodExpectationTriplet;
				if (triplet != null)
				{
					if (MockedObjectsEquality.Instance.Equals( triplet.Proxy , proxy ) &&
						triplet.Method == method &&
						triplet.Expectation == oldExpectation)
					{
						triplet.Expectation = newExpectation;
					}
				}
				//Action cannot be another recorder, since then the RemoveExpectation would've
				//passed us to the top most recorder.
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has expectations that weren't satisfied yet.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has expectations; otherwise, <c>false</c>.
		/// </value>
		protected override bool DoHasExpectations
		{
			get
			{
				foreach (object action in recordedActions)
				{
					ProxyMethodExpectationTriplet triplet = action as ProxyMethodExpectationTriplet;
					if (triplet != null)
					{
						if (triplet.Expectation.CanAcceptCalls)
							return true;
					}
					else //Action is another recorder
					{
						IMethodRecorder innerRecorder = (IMethodRecorder) action;
						if (innerRecorder.HasExpectations)
							return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Handle the real execution of this method for the derived class
		/// </summary>
		protected override void DoRemoveExpectation(IExpectation expectation)
		{
			for (int i = 0; i < recordedActions.Count; i++)
			{
				ProxyMethodExpectationTriplet triplet = recordedActions[i] as ProxyMethodExpectationTriplet;
				if (triplet != null)
				{
					if (triplet.Expectation == expectation)
						recordedActions.RemoveAt(i);
				}
				//Action cannot be another recorder, since then the RemoveExpectation would've
				//passed us to the top most recorder.
			}
		}

		/// <summary>
        /// Handles the real getting of the recorded expectation or null.
		/// </summary>
		protected override IExpectation DoGetRecordedExpectationOrNull(object proxy, MethodInfo method, object[] args)
		{
			Validate.IsNotNull(proxy, "proxy");
			Validate.IsNotNull(method, "method");
			Validate.IsNotNull(args, "args");
			// Need this because we may want to modify the recordedAction list as we traverse it
			// See: ClearReplayerToCall();
			ArrayList traversalSafeCopy = new ArrayList(recordedActions);
			bool allSatisfied = true;
			foreach (object action in traversalSafeCopy)
			{
				ProxyMethodExpectationTriplet triplet = action as ProxyMethodExpectationTriplet;
				if (triplet != null)
				{
					if (MockedObjectsEquality.Instance.Equals(triplet.Proxy, proxy) &&
						triplet.Method == method &&
						triplet.Expectation.CanAcceptCalls &&
						triplet.Expectation.IsExpected(args))
					{
						triplet.Expectation.AddActualCall();
						return triplet.Expectation;
					}
					if(!triplet.Expectation.ExpectationSatisfied)
						allSatisfied = false;
				}
				else //Action is another recorder
				{
					IMethodRecorder innerRecorder = (IMethodRecorder) action;
					if(ShouldConsiderThisReplayer(innerRecorder)==false)
						continue;
					IExpectation expectation = innerRecorder.GetRecordedExpectationOrNull(proxy, method, args);
					if (expectation != null)
					{
						replayerToCall = innerRecorder;
						return expectation;
					}
					if(innerRecorder.HasExpectations)
						allSatisfied = false;
				}
			}
			// We still have unsatisifed expectation or we don't have a parent recorder
			if (!allSatisfied || parentRecorder==null)
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
		/// Handle the real execution of this method for the derived class
		/// </summary>
		protected override void DoAddRecorder(IMethodRecorder recorder)
		{
			recordedActions.Add(recorder);
		}

		/// <summary>
		/// Gets the next expected calls string.
		/// </summary>
		public override string GetExpectedCallsMessage()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Unordered: { ");
			foreach (object action in recordedActions)
			{
				ProxyMethodExpectationTriplet triplet = action as ProxyMethodExpectationTriplet;
				if (triplet != null)
				{
					sb.Append(triplet.Expectation.ErrorMessage);
				}
				else
				{
					string nested = ((IMethodRecorder)action).GetExpectedCallsMessage();
					sb.Append(nested);
				}
			}
			sb.Append(" }");
			return sb.ToString();
		}

		/// <summary>
		/// Create an exception for an unexpected method call.
		/// </summary>
		public override ExpectationViolationException UnexpectedMethodCall(object proxy, MethodInfo method, object[] args)
		{
			// We have move to the parent recorder, we need to pass the call to it.
			if (parentRecorderRedirection != null)
				return parentRecorderRedirection.UnexpectedMethodCall(proxy, method, args);
			StringBuilder sb = new StringBuilder();
			CalcExpectedAndActual calc = new CalcExpectedAndActual(this, proxy, method, args);
			string methodAsString = MethodCallUtil.StringPresentation(method, args);
			sb.Append(methodAsString);
			sb.Append(" Expected #");
			sb.Append(calc.Expected);
			sb.Append(", Actual #").Append(calc.Actual).Append('.');
			ExpectationsList list = GetAllExpectationsForProxyAndMethod(proxy, method);
			if (list.Count > 0)
			{
				string message = list[0].Message;
				if (message != null)
				{

					sb.Append(System.Environment.NewLine)
						.Append("Message: ")
						.Append(message);
				}
			}
			AppendNextExpected(proxy, method, sb);
			ExpectationViolationException expectationViolationException = new ExpectationViolationException(sb.ToString());
			MockRepository.SetExceptionToBeThrownOnVerify(proxy, expectationViolationException);
			return expectationViolationException;
		}

		private class CalcExpectedAndActual
		{
			private int actual = 1;
			private int expected = 0;
			private UnorderedMethodRecorder parent;
			
			public int Actual
			{
				get { return actual; }
			}

			public int Expected
			{
				get { return expected; }
			}

			public CalcExpectedAndActual(UnorderedMethodRecorder parent, object proxy, MethodInfo method, object[] args)
			{
				this.parent = parent;
				Calculate(proxy, method, args);
			}

			private void Calculate(object proxy, MethodInfo method, object[] args)
			{
				ExpectationsList list = parent.GetAllExpectationsForProxyAndMethod(proxy, method);
				foreach (IExpectation expectation in list)
				{
					if (expectation.IsExpected(args))
					{
						expected++;
						actual += expectation.ActualCalls;
					}
				}
			}
		}

		private void AppendNextExpected(object proxy, MethodInfo method, StringBuilder sb)
		{
			ExpectationsList list = GetAllExpectationsForProxyAndMethod(proxy, method);
			if (list.Count > 0)
			{
				IExpectation expectation = list[0];
				if (expectation.ExpectationSatisfied)
					return; //avoid showing methods that were completed.
				sb.Append("\r\n");
				sb.Append(expectation.ErrorMessage).Append(" Expected #");
				sb.Append(expectation.Expected).Append(", Actual #");
				sb.Append(expectation.ActualCalls).Append(".");
			}
		}
	}
}