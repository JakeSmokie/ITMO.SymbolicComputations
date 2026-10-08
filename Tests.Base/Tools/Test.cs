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
        // Historical polynomial rewrites and a 20-point Taylor sample intentionally do much
        // more work than the interactive Workbench. Their tests still have explicit ceilings.
        public static EvaluationOptions ResearchLimits() => new EvaluationOptions {
            MaxNodeVisits = 100_000_000,
            MaxSteps = 1_000_000,
            MaxDuration = TimeSpan.FromSeconds(30)
        };

        public static void EvaluateAndAssert(
            Expression expression,
            Symbol expectedResult,
            ITestOutputHelper output,
            Expression context = null,
            ImmutableList<Symbol> topLevelProcessors = null,
            int? maxIterations = null,
            EvaluationOptions options = null
        ) {
            // Avoid printing megabytes of intermediate rewrites in the heavy research fixtures.
            Logger.Log = options == null ? output.WriteLine : null;

            var (steps, actual) = new SymbolicContext(context, topLevelProcessors, maxIterations)
                .Run(expression, options ?? new EvaluationOptions());

//            steps.Print(output);
//            output.WriteLine("");

//            output.WriteLine(expression.ToString());
//            output.WriteLine(actual.ToString());
//            output.WriteLine(expectedResult.ToString());

            Assert.Equal(expectedResult, actual);
        }

        public static Action<Expression, Symbol> CreateAsserter(
            ITestOutputHelper output,
            Expression context = null,
            ImmutableList<Symbol> topLevelProcessors = null,
            int? maxIterations = null,
            EvaluationOptions options = null
        ) => (expression, expected) => EvaluateAndAssert(expression, expected, output, context, topLevelProcessors, maxIterations, options);
    }
}
