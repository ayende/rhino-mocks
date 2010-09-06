using System;
using System.Reflection;
using Castle.Core.Interceptor;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
    /// <summary>
    /// Responsible for building expectations
    /// </summary>
    public class ExpectationBuilder
    {
        /// <summary>
        /// Builds the default expectation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="method">The method.</param>
        /// <param name="args">The args.</param>
        /// <param name="callCallRangeExpectation">The call call range expectation.</param>
        /// <returns></returns>
        public IExpectation BuildDefaultExpectation(IInvocation invocation, MethodInfo method, object[] args, Func<Range> callCallRangeExpectation)
        {
            var parameters = method.GetParameters();
            if (!Array.Exists(parameters, p => p.IsOut))
            {
                return new ArgsEqualExpectation(invocation, args, callCallRangeExpectation());
            }

            //The value of an incoming out parameter variable is ignored
            var constraints = new AbstractConstraint[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                constraints[i] = parameters[i].IsOut ? Is.Anything() : Is.Equal(args[i]);
            }
            return new ConstraintsExpectation(invocation, constraints, callCallRangeExpectation());
        }

        /// <summary>
        /// Builds the param expectation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public IExpectation BuildParamExpectation(IInvocation invocation, MethodInfo method)
        {
            ArgManager.CheckMethodSignature(method);
            var expectation = new ConstraintsExpectation(invocation, ArgManager.GetAllConstraints(), new Range(1, null));
            expectation.OutRefParams = ArgManager.GetAllReturnValues();
            return expectation;
        }
    }
}