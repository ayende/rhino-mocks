using System;
using System.Reflection;
using Castle.Core.Interceptor;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Utilities;
using Castle.DynamicProxy;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Validate all expectations on a mock and ignores calls to
	/// any method that was not setup properly.
	/// </summary>
	public class ReplayPartialMockState : ReplayMockState
	{
		/// <summary>
		/// Creates a new <see cref="ReplayDynamicMockState"/> instance.
		/// </summary>
		/// <param name="previousState">The previous state for this method</param>
		public ReplayPartialMockState(RecordPartialMockState previousState) : base(previousState)
		{
		}

		/// <summary>
		/// Add a method call for this state' mock.
		/// </summary>
		/// <param name="invocation">The invocation for this method</param>
		/// <param name="method">The method that was called</param>
		/// <param name="args">The arguments this method was called with</param>
		protected override object DoMethodCall(IInvocation invocation, MethodInfo method, params object[] args)
		{
			IExpectation expectation = repository.Replayer.GetRecordedExpectationOrNull(proxy, method, args);
			if (expectation != null)
				return expectation.ReturnOrThrow(invocation,args);
			else if (method.IsAbstract == false)
			{
				invocation.Proceed();
				return invocation.ReturnValue;
			}
			else
			{
				//because the expectation doesn't exist, an exception will be thrown
				return repository.Replayer.GetRecordedExpectation(invocation,proxy, method, args);
			}
		}


		/// <summary>
		/// Gets a mock state that match the original mock state of the object.
		/// </summary>
		public override IMockState BackToRecord()
		{
			return new RecordPartialMockState(proxy, repository);
		}
	}
}