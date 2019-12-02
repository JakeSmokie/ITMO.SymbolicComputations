using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;
using static ITMO.SymbolicComputations.Polynomial.SumSymbolsFunction;

namespace ITMO.SymbolicComputations.Polynomial.Tests {
    public class SymbolsSumTests {
        public SymbolsSumTests(ITestOutputHelper output) {
            evaluateAndAssert = Test.CreateAsserter(output, Seq[
                SetDelayed[SumSymbols, SumSymbolsImplementation]
            ]);
        }

        private readonly Action<Expression, Symbol> evaluateAndAssert;

        [Fact]
        public void EqualExpressionsSummed() {
            Symbol x = "x";

            evaluateAndAssert(
                SumSymbols[
                    Plus[Times[x, 3], Times[x, 3]]
                ],
                Times[x, 2, 3]
            );
        }

        [Fact]
        public void SymbolsSummed() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            evaluateAndAssert(
                SumSymbols[
                    Plus[3, x, y, 10, x, y, y, x, -1, z, y]
                ],
                Plus[Times[x, 3], Times[y, 4], z, -1, 3, 10]
            );

            evaluateAndAssert(
                SumSymbols[
                    Plus[3, 5, 6]
                ],
                14
            );
        }
    }
}