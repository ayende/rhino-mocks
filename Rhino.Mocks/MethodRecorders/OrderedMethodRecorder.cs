using System.Reflection;
using System.Text;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Utilities;
using Rhino.Mocks.Impl;

namespace Rhino.Mocks.MethodRecorders
{
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
		public OrderedMethodRecorder(IMethodRecorder parentRecorder) : base(parentRecorder)
		{
		}

		/// <summary>
		/// Creates a new <see cref="OrderedMethodRecorder"/> instance.
		/// </summary>
		public OrderedMethodRecorder()
		{
		}

		/// <summary>
        /// Handles the real getting of the recorded expectation or null.
		/// </summary>
		protected override IExpectation DoGetRecordedExpectationOrNull(object proxy, MethodInfo method, object[] args)
		{
            int actionPos = 0;
            while ( actionPos < recordedActions.Count )
            {
                ProxyMethodExpectationTriplet triplet = recordedActions[actionPos] as ProxyMethodExpectationTriplet;
                if ( triplet != null )
                {
                    if ( MockedObjectsEquality.Instance.Equals( triplet.Proxy, proxy ) &&
                        triplet.Method == method &&
                        triplet.Expectation.CanAcceptCalls &&
                        triplet.Expectation.IsExpected(args) )
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
                        if ( !triplet.Expectation.CanAcceptCalls )
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
					IMethodRecorder innerRecorder = (IMethodRecorder) recordedActions[actionPos];
					if(ShouldConsiderThisReplayer(innerRecorder)==false)
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
			if(parentRecorder==null)
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
		public override ExpectationViolationException UnexpectedMethodCall(object proxy, MethodInfo method, object[] args)
		{
			// We have move to the parent recorder, we need to pass the call to it.
			if (parentRecorderRedirection != null)
				return parentRecorderRedirection.UnexpectedMethodCall(proxy, method, args);
			StringBuilder sb = new StringBuilder();
			sb.Append("Unordered method call! The expected call is: '");
			sb.Append(GetExpectedCallsMessage());
			sb.Append("' but was: '").
				Append(MethodCallUtil.StringPresentation(method, args)).
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
				if (triplet !=  null)
				{
					if (triplet.Expectation.Message!=null)
					{
						sb.Append("Message: ")
							.Append(triplet.Expectation.Message)
							.Append(System.Environment.NewLine);
					}
					sb.Append(triplet.Expectation.ErrorMessage);
				}
				else //Action is another recorder
				{
					sb.Append(((IMethodRecorder) recordedActions[0]).GetExpectedCallsMessage());
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
