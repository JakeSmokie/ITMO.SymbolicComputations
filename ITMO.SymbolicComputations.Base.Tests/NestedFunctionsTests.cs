using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tests.Extensions;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public class NestedFunctionsTests {
        public NestedFunctionsTests(ITestOutputHelper output) =>
            _out = output;

        private void EvaluateAndAssert(Expression expression, Symbol expectedResult) {
            var (steps, actual) = expression.Visit(new FullEvaluator());
            
            steps.Print(_out);
            Assert.Equal(expectedResult, actual);
        }

        private readonly ITestOutputHelper _out;

        [Fact]
        public void TestHiddenArg() {
            Symbol x = "x";
            var expression = Fun[x, Fun[x, BinaryPlus[x, 1]]][1][2];

            EvaluateAndAssert(expression, 3);
        }
    }
}