using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.Functions.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.Functions.ListFunctions;
using static ITMO.SymbolicComputations.Polynomial.SumSymbolsFunction;

namespace ITMO.SymbolicComputations.Polynomial.Tests {
    public class SymbolsSumTests {
        public SymbolsSumTests(ITestOutputHelper output) {
            _evaluateAndAssert = Test.CreateAsserter(output);
        }

        private readonly Action<Expression, Symbol> _evaluateAndAssert;

        [Fact]
        public void SymbolsSummed() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            _evaluateAndAssert(
                SumSymbols[
                    List[3, x, y, 10, x, y, y, x, -1, z, y]
                ],
                List[Times[x, 3], Times[y, 4], z, 3, 10, -1]
            );
        }

        [Fact]
        public void EqualExpressionsSummed() {
            Symbol x = "x";

            _evaluateAndAssert(
                SumSymbols[
                    List[Times[x, 3], Times[x, 3]]
                ],
                List[Times[x, 6]]
            );
        }
    }
}