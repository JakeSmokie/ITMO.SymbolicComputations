using System;
using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Base.Tools {
    public static class Test {
        public static void EvaluateAndAssert(
            Expression expression,
            Symbol expectedResult,
            ITestOutputHelper output,
            Expression context = null,
            ImmutableList<Symbol> topLevelProcessors = null,
            int? maxIterations = null
        ) {
            Logger.Log = output.WriteLine;

            var (steps, actual) = new SymbolicContext(context, topLevelProcessors, maxIterations).Run(expression);

            steps.Print(output);
            output.WriteLine("");

            output.WriteLine(expression.ToString());
            output.WriteLine(actual.ToString());
            output.WriteLine(expectedResult.ToString());

            Assert.Equal(expectedResult, actual);
        }

        public static Action<Expression, Symbol> CreateAsserter(
            ITestOutputHelper output,
            Expression context = null,
            ImmutableList<Symbol> topLevelProcessors = null,
            int? maxIterations = null
        ) => (expression, expected) => EvaluateAndAssert(expression, expected, output, context, topLevelProcessors, maxIterations);
    }
}