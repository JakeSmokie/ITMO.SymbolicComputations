using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tests.Tools;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.Alphabet;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

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