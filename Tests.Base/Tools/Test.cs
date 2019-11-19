using System;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Base.Tools {
    public static class Test {
        public static void EvaluateAndAssert(Expression expression, Symbol expectedResult, ITestOutputHelper output) {
            var (steps, actual) = expression.Visit(new FullEvaluator());
            
            steps.Print(output);
            output.WriteLine("");

            Assert.Equal(expectedResult, actual);
        }
        
        public static Action<Expression, Symbol> CreateAsserter(ITestOutputHelper output) =>
            (expression, expected) => {
                var (steps, actual) = expression.Visit(new FullEvaluator());
            
                steps.Print(output);
                output.WriteLine("");
                
                Assert.Equal(expected, actual);
            };
    }
}