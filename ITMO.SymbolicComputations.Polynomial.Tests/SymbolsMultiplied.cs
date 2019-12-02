using System;
using ITMO.SymbolicComputations.Base.Models;
using Tests.Base.Tools;
using Xunit;
using Xunit.Abstractions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;
using static ITMO.SymbolicComputations.Polynomial.SumSymbolsFunction;
using static ITMO.SymbolicComputations.Polynomial.SymbolsTimesToPower;

namespace ITMO.SymbolicComputations.Polynomial.Tests {
    public class SymbolsMultiplied {
        public SymbolsMultiplied(ITestOutputHelper output) {
            evaluateAndAssert = Test.CreateAsserter(output, Seq[
                SetDelayed[TimesSymbols, TimesSymbolsImplementation]
            ]);
        }

        private readonly Action<Expression, Symbol> evaluateAndAssert;

        [Fact]
        public void SymbolsMultipliedInPower() {
            Symbol x = "x";
            Symbol y = "y";
            Symbol z = "z";

            evaluateAndAssert(
                TimesSymbols[
                    Times[3, x, y, 10, x, y, y, x, -1, z, y]
                ],
                Times[Power[x, 3], Power[y, 4], z, -1, 3, 10]
            );
        }
    }
}