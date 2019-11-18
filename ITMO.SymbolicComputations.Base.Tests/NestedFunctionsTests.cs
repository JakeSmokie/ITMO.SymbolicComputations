using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Functions.Alphabet;
using static ITMO.SymbolicComputations.Base.Functions.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Functions.Functions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public class NestedFunctionsTests {
        public NestedFunctionsTests(ITestOutputHelper output) =>
            _out = output;
        
        private readonly ITestOutputHelper _out;

        [Fact]
        public void TestHiddenArg() {
            var expression = Fun[x, Fun[x, BinaryPlus[x, 1]]][1][2];
            Test.EvaluateAndAssert(expression, 3, _out);
        }
    }
}