#if DOTNET35
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Generated;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks
{
	public static class RhinoMocksExtensions
	{
		[ThreadStatic] internal static IList<Expression> argumentPredicates;

		public static IMethodOptions<VoidType> Expect<T>(this T mock, Action<T> action)
		{
			return Expect<T, VoidType>(mock, t =>
			{
				action(t);
				return null;
			});
		}

		public static void BackToRecord<T>(this T mock)
		{
			BackToRecord(mock, BackToRecordOptions.All);
		}

		public static void BackToRecord<T>(this T mock, BackToRecordOptions options)
		{
			IMockedObject mockedObject = MockRepository.GetMockedObject(mock);
			mockedObject.Repository.BackToRecord(mock, options);
		}

		public static MockRepository GetMockRepository<T>(this T mock)
		{
			IMockedObject mockedObject = MockRepository.GetMockedObject(mock);
			return mockedObject.Repository;
		}

		public static IMethodOptions<R> Expect<T, R>(this T mock, Func<T, R> action)
		{
			IMockedObject mockedObject = MockRepository.GetMockedObject(mock);
			MockRepository mocks = mockedObject.Repository;
			mocks.BackToRecord(mock, BackToRecordOptions.None);
			action(mock);
			IMethodOptions<R> options = LastCall.GetOptions<R>();
			options.TentativeReturn();
			mocks.Replay(mock);
			return options;
		}

		public static IMethodOptions<object> Stub<T>(this T mock, Action<T> action)
		{
			return Stub<T, object>(mock, t =>
			{
				action(t);
				return null;
			});
		}

		public static IMethodOptions<R> Stub<T, R>(this T mock, Func<T, R> action)
		{
			return Expect(mock, action).Repeat.Times(0, 1);
		}

		public static IList<object[]> GetArgumentsForCallsMadeOn<T>(this T mock, Action<T> action)
		{
			return GetArgumentsForCallsMadeOn(mock, action, DefaultConstraintSetup);
		}

		public static IList<object[]> GetArgumentsForCallsMadeOn<T>(this T mock, Action<T> action, Action<IMethodOptions<object>> setupConstraints)
		{
			argumentPredicates = new List<Expression>();
			return GetExpectationsToVerify(mock, action, setupConstraints).ArgumentsForAllCalls;
		}

		public static void AssertWasCalled<T>(this T mock, Action<T> action)
		{
			argumentPredicates = new List<Expression>();
			AssertWasCalled(mock, action, DefaultConstraintSetup);
		}

		private static void DefaultConstraintSetup(IMethodOptions<object> options)
		{
			var constraints = new List<AbstractConstraint>();
			foreach (Expression expression in argumentPredicates)
			{
				constraints.Add(new LambdaConstraint(expression));
			}
			if (constraints.Count != 0)
				options.Constraints(constraints.ToArray());
		}

		public static void AssertWasCalled<T>(this T mock, Action<T> action, Action<IMethodOptions<object>> setupConstraints)
		{
			try
			{
				ExpectationVerificationInformation verificationInformation = GetExpectationsToVerify(mock, action, setupConstraints);

				foreach (var args in verificationInformation.ArgumentsForAllCalls)
				{
					if (verificationInformation.Expected.IsExpected(args))
						return;
				}
				throw new ExpectationViolationException("Expected that " +
				                                        verificationInformation.ExpectationsToVerify[0].ErrorMessage +
				                                        " would be called, but it was not found on the actual calls made on the mocked object.");
			}
			finally
			{
				argumentPredicates = null;
			}
		}


		public static void AssertWasNotCalled<T>(this T mock, Action<T> action)
		{
			argumentPredicates = new List<Expression>();
			AssertWasNotCalled(mock, action, delegate(IMethodOptions<object> options)
			{
				var constraints = new List<AbstractConstraint>();
				foreach (Expression expression in argumentPredicates)
				{
					constraints.Add(new LambdaConstraint(expression));
				}
				if (constraints.Count != 0)
					options.Constraints(constraints.ToArray());
			});
		}

		public static void AssertWasNotCalled<T>(this T mock, Action<T> action,
		                                         Action<IMethodOptions<object>> setupConstraints)
		{
			try
			{
				ExpectationVerificationInformation verificationInformation = GetExpectationsToVerify(mock, action, setupConstraints);

				foreach (var args in verificationInformation.ArgumentsForAllCalls)
				{
					if (verificationInformation.Expected.IsExpected(args))
						throw new ExpectationViolationException("Expected that " +
						                                        verificationInformation.ExpectationsToVerify[0].ErrorMessage +
						                                        " would not be called, but it was found on the actual calls made on the mocked object.");
				}
			}
			finally
			{
				argumentPredicates = null;
			}
		}


		private static ExpectationVerificationInformation GetExpectationsToVerify<T>(T mock, Action<T> action,
		                                                                             Action<IMethodOptions<object>>
		                                                                             	setupConstraints)
		{
			IMockedObject mockedObject = MockRepository.GetMockedObject(mock);
			MockRepository mocks = mockedObject.Repository;

			if (mocks.IsInReplayMode(mockedObject) == false)
			{
				throw new InvalidOperationException(
					"Cannot assert on an object that is not in replay mode. Did you forget to call ReplayAll() ?");
			}

			var mockToRecordExpectation =
				(T) mocks.DynamicMock(mockedObject.ImplementedTypes[0], mockedObject.ConstructorArguments);
			action(mockToRecordExpectation);

			AssertExactlySingleExpectaton(mocks, mockToRecordExpectation);

			IMethodOptions<object> lastMethodCall = mocks.LastMethodCall<object>(mockToRecordExpectation);
			lastMethodCall.TentativeReturn();
			if (setupConstraints != null)
			{
				setupConstraints(lastMethodCall);
			}
			ExpectationsList expectationsToVerify = mocks.Replayer.GetAllExpectationsForProxy(mockToRecordExpectation);
			if (expectationsToVerify.Count == 0)
				throw new InvalidOperationException(
					"The expectation was removed from the waiting expectations list, did you call Repeat.Any() ? This is not supported in AssertWasCalled()");
			IExpectation expected = expectationsToVerify[0];
			ICollection<object[]> argumentsForAllCalls = mockedObject.GetCallArgumentsFor(expected.Method);
			return new ExpectationVerificationInformation
			       	{
						ArgumentsForAllCalls = new List<object[]>(argumentsForAllCalls),
			       		ExpectationsToVerify = expectationsToVerify,
			       		Expected = expected
			       	};
		}

		public static void VerifyAllExpectations(this object mockObject)
		{
			IMockedObject mockedObject = MockRepository.GetMockedObject(mockObject);
			mockedObject.Repository.Verify(mockedObject);
		}

		private static void AssertExactlySingleExpectaton<T>(MockRepository mocks, T mockToRecordExpectation)
		{
			if (mocks.Replayer.GetAllExpectationsForProxy(mockToRecordExpectation).Count == 0)
				throw new InvalidOperationException(
					"No expectations were setup to be verified, ensure that the method call in the action is a virtual (C#) / overridable (VB.Net) method call");

			if (mocks.Replayer.GetAllExpectationsForProxy(mockToRecordExpectation).Count > 1)
				throw new InvalidOperationException(
					"You can only use a single expectation on AssertWasCalled(), use separate calls to AssertWasCalled() if you want to verify several expectations");
		}

		#region Nested type: VoidType

		public class VoidType
		{
			private VoidType()
			{
			}
		}

		#endregion
	}
}

#endif