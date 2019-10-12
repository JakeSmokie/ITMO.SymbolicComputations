using ITMO.SymbolicComputations.Base.Models;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public sealed class EvaluationTests {
        public EvaluationTests(ITestOutputHelper output) =>
            _out = output;

        private readonly ITestOutputHelper _out;

        [Fact]
        public void EvaluationOfTwoPlusThreeIsOkay() {
            var function = Evaluate[Plus[2, 3]];
        }
    }
}