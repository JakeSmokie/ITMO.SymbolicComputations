using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Predefined.ListFunctions;
using static ITMO.SymbolicComputations.Polynomial.SumSymbolsFunction;

namespace ITMO.SymbolicComputations.Polynomial.Tests {
    public class SymbolsSumTests {
        public SymbolsSumTests(ITestOutputHelper output) {
            _evaluateAndAssert = Test.EvaluateAndAssert(output);
        }

        private readonly Action<Expression, Symbol> _evaluateAndAssert;

        [Fact]
        public void SymbolsSummedInTimes() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            _evaluateAndAssert(
                SumSymbols[
                    List[3, x, y, 10, x, y, y, x, -1, z, y]
                ],
                List[BinaryTimes[3, x], BinaryTimes[4, y], z, 3, 10, -1]
            );
        }
    }
}