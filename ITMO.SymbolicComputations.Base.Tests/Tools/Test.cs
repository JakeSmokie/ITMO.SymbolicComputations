using System;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace ITMO.SymbolicComputations.Base.Tests.Tools {
    public static class Test {
        public static Action<Expression, Symbol> EvaluateAndAssert(ITestOutputHelper output) =>
            (expression, expected) => {
                var (steps, actual) = expression.Visit(new FullEvaluator());
            
                steps.Print(output);
                Assert.Equal(expected, actual);
            };
    }
}